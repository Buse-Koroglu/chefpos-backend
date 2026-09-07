using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Orders.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Orders.Commands.AddOrderItem;

public class AddOrderItemCommandHandler : IRequestHandler<AddOrderItemCommand, OrderResponseDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public AddOrderItemCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<OrderResponseDto> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
    {
        var requestingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken);
        if (requestingUser is null)
            throw new NotFoundException("Kullanıcı bulunamadı.");

        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken).OrThrowNotFoundAsync($"Sipariş bulunamadı: {request.OrderId}");

        var isCashierHere = requestingUser.HasRoleAtLocation(Role.CASHIER, order.LocationId);
        var isOwningWaiter = requestingUser.HasRoleAtLocation(Role.WAITER, order.LocationId) && order.CreatedByUserId == requestingUser.Id;
        if (!isCashierHere && !isOwningWaiter)
            throw new ForbiddenException("Bu sipariş üzerinde işlem yapma yetkiniz yok.");

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken).OrThrowNotFoundAsync($"Ürün bulunamadı: {request.ProductId}");
        order.AddItem(request.ProductId, request.Quantity, product.Price, product.Name);
        await _orderRepository.SaveAllChangesAsync(cancellationToken);
        return OrderResponseDto.FromEntity(order);
    }
}
