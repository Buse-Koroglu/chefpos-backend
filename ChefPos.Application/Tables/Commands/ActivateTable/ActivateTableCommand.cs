using ChefPos.Application.Tables.DTOs;
using MediatR;

namespace ChefPos.Application.Tables.Commands.ActivateTable;

public class ActivateTableCommand : IRequest<TableResponseDto>
{
    public Guid TableId { get; }

    public ActivateTableCommand(Guid tableId)
    {
        TableId = tableId;
    }
}
