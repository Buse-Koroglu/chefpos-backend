using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Common.Pagination;
using ChefPos.Application.Orders.DTOs;
using MediatR;

namespace ChefPos.Application.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PagedResult<OrderResponseDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILocationRepository _locationRepository;

        public GetOrdersQueryHandler(
            IOrderRepository orderRepository,
            ILocationRepository locationRepository)
        {
            _orderRepository = orderRepository;
            _locationRepository = locationRepository;
        }

        public async Task<PagedResult<OrderResponseDto>> Handle(
            GetOrdersQuery request,
            CancellationToken cancellationToken)
        {
            await _locationRepository
                .GetByIdAsync(request.LocationId, cancellationToken)
                .OrThrowNotFoundAsync(
                    $"Yerleşke bulunamadı: {request.LocationId}");

            var (orders, totalCount) =
                await _orderRepository.GetAllByLocationPagedAsync(
                    request.LocationId,
                    request.Status,
                    request.Type,
                    request.PaymentStatus,
                    request.SearchTerm,
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);

            return new PagedResult<OrderResponseDto>
            {
                Items = orders
                    .Select(OrderResponseDto.FromEntity)
                    .ToList(),

                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }

