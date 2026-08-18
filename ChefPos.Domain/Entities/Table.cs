using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class Table : BaseEntity
{
    public int TableNumber { get; private set; }
    public Guid LocationId { get; private set; }
    public Location Location { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;

    private Table() { }

    public Table(Guid locationId, int tableNumber)
    {
        if (tableNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tableNumber), "Masa numarası pozitif olmalı.");
        }

        LocationId = locationId;
        TableNumber = tableNumber;
    }

    public void UpdateTableNumber(int tableNumber)
    {
        if (tableNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tableNumber), "Masa numarası pozitif olmalı.");
        }

        TableNumber = tableNumber;
        Touch();
    }

    public void Activate() { IsActive = true; Touch(); }
    public void Deactivate() { IsActive = false; Touch(); }
}
