using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Orders.DTOs;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderResponseDto>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository  _userRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ITableRepository _tableRepository;

    public CreateOrderCommandHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ITableRepository tableRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _tableRepository = tableRepository;
    }

    public async Task<OrderResponseDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        var requestingUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);
        if (requestingUser is null)
        {
            throw new NotFoundException("Kullanıcı bulunamadı.");
        }
        
        Order order;
        switch (request.RequestedAs)
        {
            case Role.CASHIER when requestingUser.HasRoleAtLocation(Role.CASHIER, request.LocationId):
                order = Order.CreateByCashier(request.LocationId, currentUserId, request.CustomerName!);
                break;
            case Role.WAITER when requestingUser.HasRoleAtLocation(Role.WAITER, request.LocationId):
                if (request.IsPackage)
                {
                    order = Order.CreatePackageByWaiter(request.LocationId, currentUserId, request.CustomerName!);
                    break;
                }

                if (request.TableId is null)
                {
                    throw new ValidationException("Garson siparişlerinde masa seçimi zorunludur.");
                }
                var table = await _tableRepository.GetByIdAsync(request.TableId.Value, cancellationToken)
                    .OrThrowNotFoundAsync($"Masa bulunamadı: {request.TableId}");

                var existingOpenOrder = await _orderRepository.GetOpenOrderByTableIdAsync(table.Id, cancellationToken);
                if (existingOpenOrder is not null)
                {
                    throw new ValidationException(
                        $"Bu masanın ödemesi alınmadan yeni bir sipariş oluşturulamaz: Masa {table.TableNumber}",
                        "TABLE_OCCUPIED");
                }

                order = Order.CreateByWaiter(request.LocationId, currentUserId, request.CustomerName!, table);
                break;
            default:
                throw new ValidationException(
                    $"Kullanıcının '{request.RequestedAs}' rolü olarak sipariş oluşturma yetkisi yok.");
        }
 
        foreach (var itemRequest in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemRequest.ProductId, cancellationToken);
            if (product is null)
            {
                throw new NotFoundException("Ürün bulunamadı");
            }
 
            order.AddItem(product.Id, itemRequest.Quantity, product.Price, product.Name);
        }
 
        await _orderRepository.AddAsync(order, cancellationToken);
        await _orderRepository.SaveAllChangesAsync(cancellationToken);
 
       
        return OrderResponseDto.FromEntity(order);
 
    }
}
