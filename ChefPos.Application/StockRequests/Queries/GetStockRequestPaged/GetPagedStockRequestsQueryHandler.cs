using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Common.Pagination;
using ChefPos.Application.StockRequests.DTOs;
using MediatR;

namespace ChefPos.Application.StockRequests.Queries.GetStockRequestPaged;

public class GetPagedStockRequestsQueryHandler
    : IRequestHandler<GetPagedStockRequestsQuery, PagedResult<AdminStockRequestResponseDto>>
{
    private readonly IStockRequestRepository _stockRequestRepository;

    public GetPagedStockRequestsQueryHandler(IStockRequestRepository stockRequestRepository)
    {
        _stockRequestRepository = stockRequestRepository;
    }

    public async Task<PagedResult<AdminStockRequestResponseDto>> Handle(
        GetPagedStockRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _stockRequestRepository.GetAllPagedAsync(
            request.SearchTerm,
            request.LocationId,
            request.Status,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(AdminStockRequestResponseDto.FromEntity).ToList();

        return new PagedResult<AdminStockRequestResponseDto>
        {
            Items = dtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}