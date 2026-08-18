using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Common.Pagination;
using ChefPos.Application.Tables.DTOs;
using MediatR;

namespace ChefPos.Application.Tables.Queries.GetTablesPaged;

public class GetTablesPagedQueryHandler : IRequestHandler<GetTablesPagedQuery, PagedResult<TableResponseDto>>
{
    private readonly ITableRepository _tableRepository;

    public GetTablesPagedQueryHandler(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public async Task<PagedResult<TableResponseDto>> Handle(GetTablesPagedQuery request, CancellationToken cancellationToken)
    {
        var (tables, totalCount) = await _tableRepository.GetAllPagedAsync(
            request.SearchTerm, request.LocationId, request.IsActive, request.PageNumber, request.PageSize, cancellationToken);

        return new PagedResult<TableResponseDto>
        {
            Items = tables.Select(TableResponseDto.FromEntity).ToList(),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
        };
    }
}
