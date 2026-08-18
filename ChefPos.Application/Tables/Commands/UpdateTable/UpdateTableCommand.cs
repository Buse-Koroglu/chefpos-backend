using ChefPos.Application.Tables.DTOs;
using MediatR;

namespace ChefPos.Application.Tables.Commands.UpdateTable;

public class UpdateTableCommand : IRequest<TableResponseDto>
{
    public Guid TableId { get; set; }
    public int TableNumber { get; set; }

    public UpdateTableCommand(Guid tableId, int tableNumber)
    {
        TableId = tableId;
        TableNumber = tableNumber;
    }
}
