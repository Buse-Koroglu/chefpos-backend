using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Common.Pagination;
using ChefPos.Application.Orders.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PagedResult<OrderResponseDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetOrdersQueryHandler(IOrderRepository orderRepository,ILocationRepository locationRepository, IUserRepository userRepository, ICurrentUserService currentUserService)
        {
            _orderRepository = orderRepository;
            _locationRepository = locationRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<OrderResponseDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken).OrThrowNotFoundAsync($"Yerleşke bulunamadı: {request.LocationId}");

            var requestingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken);
            if (requestingUser is null)
                throw new NotFoundException("Kullanıcı bulunamadı.");

            // Sadece garson kendi siprişlerini görür
            // kasiyer/admin/mutfak yerleşkedeki tüm siparişleri görür
            var isWaiterOnly = requestingUser.HasRole(Role.WAITER) && !requestingUser.HasRole(Role.CASHIER) && !requestingUser.HasRole(Role.ADMIN) && !requestingUser.HasRole(Role.KITCHEN) && !requestingUser.HasRole(Role.SUPER_ADMIN);

            var createdByUserId = isWaiterOnly ? requestingUser.Id : request.CreatedByUserId;

            var (orders, totalCount) =
                await _orderRepository.GetAllByLocationPagedAsync(request.LocationId, request.Status, request.Type, request.PaymentStatus, request.SearchTerm, createdByUserId, request.FromDate, request.ToDate, request.PageNumber, request.PageSize, cancellationToken);

            return new PagedResult<OrderResponseDto>
            {
                Items = orders.Select(OrderResponseDto.FromEntity).ToList(),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }

