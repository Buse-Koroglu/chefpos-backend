using ChefPos.Domain.Common;
using ChefPos.Domain.Enums;

namespace ChefPos.Domain.Entities;

public class StockRequest : BaseEntity
{
    public Guid IngredientId { get; private set; }
    public Ingredient Ingredient { get; private set; } = null!;

    public Guid LocationId { get; private set; }
    public Location Location { get; private set; } = null!;

    public Guid RequestedByUserId { get; private set; }
    public User RequestedByUser { get; private set; } = null!;

    public decimal RequestedQuantity { get; private set; }
    public StockRequestStatus Status { get; private set; }

    public Guid? DecidedByUserId { get; private set; }
    public User? DecidedByUser { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTime? DecidedAt { get; private set; }
    public decimal? ApprovedUnitPrice { get; private set; }

    private StockRequest() { }

    public StockRequest(Guid ingredientId, Guid locationId, Guid requestedByUserId, decimal requestedQuantity)
    {
        if (requestedQuantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(requestedQuantity), "Talep edilen miktar pozitif olmalı.");
        }

        IngredientId = ingredientId;
        LocationId = locationId;
        RequestedByUserId = requestedByUserId;
        RequestedQuantity = requestedQuantity;
        Status = StockRequestStatus.PENDING;
    }

    public void Approve(Guid decidedByUserId, decimal approvedUnitPrice)
    {
        EnsurePending();

        if (approvedUnitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(approvedUnitPrice), "Fiyat negatif olamaz.");
        }

        Status = StockRequestStatus.APPROVED;
        DecidedByUserId = decidedByUserId;
        DecidedAt = DateTime.UtcNow;
        ApprovedUnitPrice = approvedUnitPrice;
        Touch();
    }

    public void Reject(Guid decidedByUserId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Red gerekçesi boş olamaz.", nameof(reason));
        }

        EnsurePending();

        Status = StockRequestStatus.REJECTED;
        DecidedByUserId = decidedByUserId;
        RejectionReason = reason;
        DecidedAt = DateTime.UtcNow;
        Touch();
    }

    private void EnsurePending()
    {
        if (Status != StockRequestStatus.PENDING)
        {
            throw new InvalidOperationException("Sadece bekleyen stok talepleri onaylanabilir/reddedilebilir.");
        }
    }
}