using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Orders.DTOs;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Orders.Commands.CompleteOrder;

public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommand,OrderResponseDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStockMovementRepository _stockMovementRepository;

    public CompleteOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IStockMovementRepository stockMovementRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _stockMovementRepository = stockMovementRepository;
    }
    
    public async Task<OrderResponseDto> Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
    {
        var requestingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken);
        if (requestingUser is null)
            throw new NotFoundException("Kullanıcı bulunamadı.");

        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            .OrThrowNotFoundAsync($"Sipariş bulunamadı : {request.OrderId}");

        var canComplete = order.OrderType switch
        {
            OrderType.CASHIER => requestingUser.HasRoleAtLocation(Role.CASHIER, order.LocationId),
            OrderType.WAITER or OrderType.SELF_SERVICE => requestingUser.HasRoleAtLocation(Role.KITCHEN, order.LocationId),
            _ => false
        };

        if (!canComplete)
            throw new ForbiddenException("Bu siparişi tamamlama yetkiniz yok.");

        order.Complete();

        var productCache = new Dictionary<Guid, Product>();
        foreach (var item in order.Items)
        {
            if (item.ProductId is null)
            {
                continue;
            }
            if (!productCache.TryGetValue(item.ProductId.Value, out var product))
            {
                product = await _productRepository.GetByIdAsync(item.ProductId.Value, cancellationToken);
                if (product is null)
                {
                    throw new NotFoundException($"Ürün bulunamadı: {item.ProductId.Value}");
                }
                productCache[item.ProductId.Value] = product;
            }
            var productLocation = product.ProductLocations.FirstOrDefault(pl => pl.LocationId == order.LocationId);
            if (productLocation is null)
            {
                throw new ValidationException($"'{product.Name}' ürünü bu siparişin yerleşkesinde tanımlı değil.");
            }

            foreach (var recipeLine in productLocation.ProductItems)
            {
                var amountToDeduct = item.Quantity * recipeLine.QuantityPerServing;
                var ingredient = recipeLine.Ingredient;
                var consumptions = ingredient.DeductStockFifo(amountToDeduct);
                var movement = StockMovement.CreateOrderSaleDeduction(ingredient.Id, ingredient.LocationId, requestingUser.Id, order.Id, consumptions);
                await _stockMovementRepository.AddAsync(movement, cancellationToken);
            }
        }
        await _orderRepository.SaveAllChangesAsync(cancellationToken);
        return OrderResponseDto.FromEntity(order);
    }
}