using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.StockRequests.DTOs;
using ChefPos.Application.StockRequests.Queries.GetStockRequestById;
using MediatR;

public class GetStockRequestByIdQueryHandler : IRequestHandler<GetStockRequestByIdQuery, StockRequestResponseDto>
{
    private readonly IStockRequestRepository _stockRequestRepository;

    public GetStockRequestByIdQueryHandler(IStockRequestRepository stockRequestRepository) => _stockRequestRepository = stockRequestRepository;

    public async Task<StockRequestResponseDto> Handle(GetStockRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var stockRequest = await _stockRequestRepository.GetByIdAsync(request.StockRequestId, cancellationToken).OrThrowNotFoundAsync($"Stok talebi bulunamadı: {request.StockRequestId}");

        return StockRequestResponseDto.FromEntity(stockRequest);
    }
}