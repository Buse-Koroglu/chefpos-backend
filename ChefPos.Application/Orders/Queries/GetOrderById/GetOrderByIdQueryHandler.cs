using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Orders.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderResponseDto>
{

    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository, IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<OrderResponseDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken).OrThrowNotFoundAsync($"Sipariş bulunamadı: {request.OrderId}");

        var requestingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken);
        if (requestingUser is null)
            throw new NotFoundException("Kullanıcı bulunamadı.");

        var hasLocationAccess = requestingUser.HasRoleAtLocation(Role.CASHIER, order.LocationId)
            || requestingUser.HasRoleAtLocation(Role.WAITER, order.LocationId)
            || requestingUser.HasRoleAtLocation(Role.ADMIN, order.LocationId);
        if (!hasLocationAccess)
            throw new ForbiddenException("Bu siparişi görüntüleme yetkiniz yok.");

        var isWaiterOnly = requestingUser.HasRole(Role.WAITER) && !requestingUser.HasRole(Role.CASHIER) && !requestingUser.HasRole(Role.ADMIN) && !requestingUser.HasRole(Role.KITCHEN) && !requestingUser.HasRole(Role.SUPER_ADMIN);

        if (isWaiterOnly && order.CreatedByUserId != requestingUser.Id)
            throw new ForbiddenException("Bu siparişi görüntüleme yetkiniz yok.");

        return OrderResponseDto.FromEntity(order);
    }

}

    
