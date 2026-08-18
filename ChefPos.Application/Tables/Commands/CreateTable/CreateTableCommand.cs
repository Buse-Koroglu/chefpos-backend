using ChefPos.Application.Tables.DTOs;
using MediatR;

namespace ChefPos.Application.Tables.Commands.CreateTable;

public class CreateTableCommand : IRequest<TableResponseDto>
{
    public Guid LocationId { get; set; }
    public int TableNumber { get; set; }
}
