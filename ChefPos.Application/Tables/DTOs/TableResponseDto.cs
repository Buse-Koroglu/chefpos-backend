using ChefPos.Domain.Entities;

namespace ChefPos.Application.Tables.DTOs;

public class TableResponseDto
{
    public Guid Id { get; set; }
    public int TableNumber { get; set; }
    public Guid LocationId { get; set; }
    public bool IsActive { get; set; }

    public static TableResponseDto FromEntity(Table table)
    {
        return new TableResponseDto
        {
            Id = table.Id,
            TableNumber = table.TableNumber,
            LocationId = table.LocationId,
            IsActive = table.IsActive,
        };
    }
}
