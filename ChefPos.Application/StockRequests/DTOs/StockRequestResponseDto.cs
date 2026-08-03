using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;

namespace ChefPos.Application.StockRequests.DTOs;

public class StockRequestResponseDto
{
    public Guid Id { get; set; }
    public Guid IngredientId { get; set; }
    public string IngredientName { get; set; } = default!;
    public Guid LocationId { get; set; }
    public Guid RequestedByUserId { get; set; }
    public decimal RequestedQuantity { get; set; }
    public StockRequestStatus Status { get; set; }
    public Guid? DecidedByUserId { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? DecidedAt { get; set; }
 
    public static StockRequestResponseDto FromEntity(StockRequest stockRequest)
    {
        return new StockRequestResponseDto
        {
            Id = stockRequest.Id,
            IngredientId = stockRequest.IngredientId,
            IngredientName = stockRequest.Ingredient.Name,
            LocationId = stockRequest.LocationId,
            RequestedByUserId = stockRequest.RequestedByUserId,
            RequestedQuantity = stockRequest.RequestedQuantity,
            Status = stockRequest.Status,
            DecidedByUserId = stockRequest.DecidedByUserId,
            RejectionReason = stockRequest.RejectionReason,
            DecidedAt = stockRequest.DecidedAt
        };
    }
}
