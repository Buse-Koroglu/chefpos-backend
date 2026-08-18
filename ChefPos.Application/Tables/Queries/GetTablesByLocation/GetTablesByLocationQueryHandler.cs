using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Tables.DTOs;
using MediatR;

namespace ChefPos.Application.Tables.Queries.GetTablesByLocation;

public class GetTablesByLocationQueryHandler : IRequestHandler<GetTablesByLocationQuery, List<TableResponseDto>>
{
    private readonly ITableRepository _tableRepository;

    public GetTablesByLocationQueryHandler(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public async Task<List<TableResponseDto>> Handle(GetTablesByLocationQuery request, CancellationToken cancellationToken)
    {
        var tables = await _tableRepository.GetAllByLocationAsync(request.LocationId, request.IncludeInactive, cancellationToken);

        return tables.Select(TableResponseDto.FromEntity).ToList();
    }
}
