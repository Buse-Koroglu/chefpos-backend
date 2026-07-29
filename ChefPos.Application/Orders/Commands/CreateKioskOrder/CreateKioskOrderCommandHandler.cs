using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Orders.DTOs;
using ChefPos.Domain.Entities;
using MediatR;

namespace ChefPos.Application.Orders.Commands.CreateKioskOrder;

public class CreateKioskOrderCommandHandler : IRequestHandler<CreateKioskOrderCommand,OrderResponseDto>
{
    private readonly IOrderRepository  _orderRepository;
    private readonly IProductRepository _productRepository;

    public CreateKioskOrderCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<OrderResponseDto> Handle(CreateKioskOrderCommand request, CancellationToken cancellationToken)
    {
        var order = Order.CreateByKiosk(request.LocationId, request.CustomerName!);

        foreach (var itemRequest in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemRequest.ProductId, cancellationToken).OrThrowNotFoundAsync($"Ürün bulunamadı: {itemRequest.ProductId}");

            order.AddItem(product.Id, itemRequest.Quantity, product.Price, product.Name);
        }
        await _orderRepository.AddAsync(order, cancellationToken);
        await _orderRepository.SaveAllChangesAsync(cancellationToken);
        
        return OrderResponseDto.FromEntity(order);
    }
}