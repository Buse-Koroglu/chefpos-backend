using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Orders.DTOs;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderResponseDto>
{
    private readonly IUserRepository  _userRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public CreateOrderCommandHandler(
        IUserRepository userRepository,
        IOrderRepository orderRepository,
        IProductRepository productRepository)
    {
        _userRepository = userRepository;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<OrderResponseDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var requestingUser = await _userRepository.GetByIdAsync(request.CreatedByUserId, cancellationToken);
        if (requestingUser is null)
        {
            throw new InvalidOperationException("Kullanıcı bulunamadı.");
        }
        
        if (!requestingUser.HasAccessToLocation(request.LocationId))
            throw new UnauthorizedAccessException("Bu kullanıcının belirtilen yerleşkede işlem yapma yetkisi yok.");
 
        var order = requestingUser.Role switch
        {
            Role.CASHIER => Order.CreateByCashier(request.LocationId, request.CreatedByUserId, request.CustomerName!),
            Role.WAITER => Order.CreateByWaiter(request.LocationId, request.CreatedByUserId, request.CustomerName!),
            _ => throw new InvalidOperationException($"'{requestingUser.Role}' rolündeki bir kullanıcı sipariş oluşturamaz.")
        };
 
        foreach (var itemRequest in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemRequest.ProductId, cancellationToken);
            if (product is null)
            {
                throw new InvalidOperationException("Ürün bulunamadı");
            }
 
            order.AddItem(product.Id, itemRequest.Quantity, product.Price, product.Name);
        }
 
        await _orderRepository.AddAsync(order, cancellationToken);
        await _orderRepository.SaveAllChangesAsync(cancellationToken);
 
       
        return OrderResponseDto.FromEntity(order);
 
    }
}

