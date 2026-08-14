using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Orders.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Orders.Commands.MakePaidOrder;

public class MakePaidOrderCommandHandler : IRequestHandler<MakePaidOrderCommand,OrderResponseDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public MakePaidOrderCommandHandler(
        IOrderRepository orderRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<OrderResponseDto> Handle(MakePaidOrderCommand request, CancellationToken cancellationToken)
    {
        var requestingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken);
        if (requestingUser is null)
            throw new NotFoundException("Kullanıcı bulunamadı.");

        if (!requestingUser.HasRole(Role.CASHIER))
            throw new ForbiddenException("Ödeme işlemini yalnızca kasiyer gerçekleştirebilir.");

        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            .OrThrowNotFoundAsync($"Sipariş bulunamadı: {request.OrderId}");

        if (!requestingUser.HasAccessToLocation(order.LocationId))
            throw new ForbiddenException("Bu kullanıcının belirtilen yerleşkede işlem yapma yetkisi yok.");

        order.MarkAsPaid();
        await _orderRepository.SaveAllChangesAsync(cancellationToken);
        return OrderResponseDto.FromEntity(order);
    }
}