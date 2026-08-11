using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Orders.DTOs;
using ChefPos.Application.Orders.Queries.CashierDashboard;
using MediatR;

public class GetCashierDashboardQueryHandler : IRequestHandler<GetCashierDashboardQuery, CashierDashboardResponseDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public GetCashierDashboardQueryHandler(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<CashierDashboardResponseDto> Handle(GetCashierDashboardQuery request, CancellationToken cancellationToken)
    {
        var pendingOrdersCount = await _orderRepository.GetPendingOrdersCountAsync(request.LocationId, cancellationToken);
        var todayRevenue = await _orderRepository.GetTodayRevenueAsync(request.LocationId, cancellationToken);
        var bestSeller = await _orderRepository.GetBestSellingProductAsync(request.LocationId, cancellationToken);

        BestSellingProductDto? bestSellerDto = null;
        if (bestSeller.HasValue)
        {
            var product = await _productRepository.GetByIdAsync(bestSeller.Value.ProductId, cancellationToken);
            bestSellerDto = new BestSellingProductDto
            {
                ProductId = bestSeller.Value.ProductId,
                ProductName = product?.Name ?? "Bilinmeyen Ürün",
                TotalQuantitySold = bestSeller.Value.TotalQuantitySold
            };
        }

        return new CashierDashboardResponseDto
        {
            PendingOrdersCount = pendingOrdersCount,
            TodayRevenue = todayRevenue,
            BestSellingProduct = bestSellerDto
        };
    }
}