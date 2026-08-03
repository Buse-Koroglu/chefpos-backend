using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.StockRequests.DTOs;
using MediatR;

namespace ChefPos.Application.StockRequests.Queries.GetStockRequests;

public class GetStockRequestsQueryHandler : IRequestHandler<GetStockRequestsQuery, List<StockRequestResponseDto>>
{
    private readonly IStockRequestRepository _stockRequestRepository;

    public GetStockRequestsQueryHandler(IStockRequestRepository stockRequestRepository) => _stockRequestRepository = stockRequestRepository;

    public async Task<List<StockRequestResponseDto>> Handle(GetStockRequestsQuery request, CancellationToken cancellationToken)
    {
        var stockRequests = await _stockRequestRepository.GetAllByLocationAsync(request.LocationId, request.Status, cancellationToken);

        return stockRequests.Select(StockRequestResponseDto.FromEntity).ToList();
    }
}