using ChefPos.Application.Common.Export;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Users.Queries.ExportUsers;

public class ExportUsersQuery : IRequest<ExportFileResult>
{
    public string? SearchTerm { get; }
    public Role? Role { get; }
    public bool? IsActive { get; }
    public Guid? LocationId { get; }

    public ExportUsersQuery(string? searchTerm, Role? role, bool? isActive, Guid? locationId)
    {
        SearchTerm = searchTerm;
        Role = role;
        IsActive = isActive;
        LocationId = locationId;
    }
}
