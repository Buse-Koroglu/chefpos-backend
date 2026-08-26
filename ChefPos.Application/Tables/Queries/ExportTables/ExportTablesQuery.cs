using ChefPos.Application.Common.Export;
using MediatR;

namespace ChefPos.Application.Tables.Queries.ExportTables;

public class ExportTablesQuery : IRequest<ExportFileResult>
{
    public string? SearchTerm { get; }
    public Guid? LocationId { get; }
    public bool? IsActive { get; }

    public ExportTablesQuery(string? searchTerm, Guid? locationId, bool? isActive)
    {
        SearchTerm = searchTerm;
        LocationId = locationId;
        IsActive = isActive;
    }
}
