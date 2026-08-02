using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Orders.DTOs;
using MediatR;

namespace ChefPos.Application.Orders.Commands.CompleteOrder;

public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommand,OrderResponseDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public CompleteOrderCommandHandler(IOrderRepository orderRepository,IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }
    
    public async Task<OrderResponseDto> Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken).OrThrowNotFoundAsync($"Sipariş bulunamadı : {request.OrderId}");
        order.Complete();

        foreach (var item in order.Items)
        {
            if (item.ProductId is null)
            {
                continue;
            }

            var product = await _productRepository.GetByIdAsync(item.ProductId.Value, cancellationToken).OrThrowNotFoundAsync($"Ürün bulunamadı: {item.ProductId}");
            foreach (var recipeLine in product.ProductItems)
            {
                var amoutnToDeduct = item.Quantity * recipeLine.QuantityPerServing;
                recipeLine.Ingredient.DecreaseStock(amoutnToDeduct);
            }
        }
        await _orderRepository.SaveAllChangesAsync(cancellationToken);
        return OrderResponseDto.FromEntity(order);
    }
}