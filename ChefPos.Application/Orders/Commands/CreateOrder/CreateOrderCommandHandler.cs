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

    public CreateOrderCommandHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        IOrderRepository orderRepository,
        IProductRepository productRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<OrderResponseDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        var requestingUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);
        if (requestingUser is null)
        {
            throw new NotFoundException("Kullanıcı bulunamadı.");
        }
        
        if (!requestingUser.HasAccessToLocation(request.LocationId))
            throw new ForbiddenException("Bu kullanıcının belirtilen yerleşkede işlem yapma yetkisi yok.");
 
        var order = request.RequestedAs switch
        {
            Role.CASHIER when requestingUser.HasRole(Role.CASHIER)
                => Order.CreateByCashier(request.LocationId, currentUserId, request.CustomerName!),
            Role.WAITER when requestingUser.HasRole(Role.WAITER)
                => Order.CreateByWaiter(request.LocationId, currentUserId, request.CustomerName!),
            _ => throw new ValidationException(
                $"Kullanıcının '{request.RequestedAs}' rolü olarak sipariş oluşturma yetkisi yok.")
        };
 
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
