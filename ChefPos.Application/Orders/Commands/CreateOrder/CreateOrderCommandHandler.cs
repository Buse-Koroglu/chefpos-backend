using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Orders.DTOs;
using ChefPos.Domain.Entities;
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
        var cashier = await _userRepository.GetByIdAsync(request.CashierId, cancellationToken);
        if (cashier is null)
        {
            throw new InvalidOperationException("Kasiyer bulunamadı.");
        }
        
        if (!cashier.HasAccessToLocation(request.LocationId))
            throw new UnauthorizedAccessException("Bu kasiyerin belirtilen yerleşkede işlem yapma yetkisi yok.");


        var order = Order.CreateByCashier(request.LocationId, request.CashierId, request.CustomerName!);

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

    
