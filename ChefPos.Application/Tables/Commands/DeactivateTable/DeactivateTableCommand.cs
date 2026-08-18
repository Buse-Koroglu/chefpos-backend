using ChefPos.Application.Tables.DTOs;
using MediatR;

namespace ChefPos.Application.Tables.Commands.DeactivateTable;

public class DeactivateTableCommand : IRequest<TableResponseDto>
{
    public Guid TableId { get; }

    public DeactivateTableCommand(Guid tableId)
    {
        TableId = tableId;
    }
}
