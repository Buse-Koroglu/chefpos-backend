using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Orders.DTOs;
using ChefPos.Domain.Entities;
using MediatR;

namespace ChefPos.Application.Orders.Commands.CreateKioskOrder;

public class CreateKioskOrderCommandHandler : IRequestHandler<CreateKioskOrderCommand,OrderResponseDto>
{
    private readonly IOrderRepository  _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILocationRepository _locationRepository;
 
    public CreateKioskOrderCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository, ILocationRepository locationRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _locationRepository = locationRepository;
    }
 
    public async Task<OrderResponseDto> Handle(CreateKioskOrderCommand request, CancellationToken cancellationToken)
    {
        var location = await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken);
        if (location is null)
        {
            throw new NotFoundException("Yerleşke bulunamadı.");
        }

        var order = Order.CreateByKiosk(request.LocationId, request.CustomerName!);
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