using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Orders.DTOs;
using MediatR;

namespace ChefPos.Application.Orders.Commands.AddOrderItem;

public class AddOrderItemCommandHandler : IRequestHandler<AddOrderItemCommand, OrderResponseDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public AddOrderItemCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<OrderResponseDto> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken).OrThrowNotFoundAsync($"Sipariş bulunamadı: {request.OrderId}");
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken).OrThrowNotFoundAsync($"Ürün bulunamadı: {request.ProductId}");
        order.AddItem(request.ProductId, request.Quantity, product.Price, product.Name);
        await _orderRepository.SaveAllChangesAsync(cancellationToken);
        return OrderResponseDto.FromEntity(order);
    }
}