using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;

namespace ChefPos.Application.StockRequests.DTOs;

public class AdminStockRequestResponseDto
{
    public Guid Id { get; set; }
    public Guid IngredientId { get; set; }
    public string IngredientName { get; set; } = default!;
    public StockUnit Unit { get; set; }
    
    public Guid LocationId { get; set; }
    public string LocationName { get; set; } = default!;
    
    public decimal RequestedQuantity { get; set; }
    public StockRequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public Guid RequestedByUserId { get; set; }
    public string RequestedByUserName { get; set; } = default!;
    
    public Guid? DecidedByUserId { get; set; }
    public string? DecidedByUserName { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? DecidedAt { get; set; }

    public static AdminStockRequestResponseDto FromEntity(StockRequest stockRequest)
    {
        return new AdminStockRequestResponseDto
        {
            Id = stockRequest.Id,
            IngredientId = stockRequest.IngredientId,
            IngredientName = stockRequest.Ingredient?.Name ?? string.Empty,
            Unit = stockRequest.Ingredient?.Unit ?? default(StockUnit),
            LocationId = stockRequest.LocationId,
            LocationName = stockRequest.Location?.Name ?? string.Empty,
            RequestedQuantity = stockRequest.RequestedQuantity,
            Status = stockRequest.Status,
            CreatedAt = stockRequest.CreatedAt,
            RequestedByUserId = stockRequest.RequestedByUserId,
            RequestedByUserName = stockRequest.RequestedByUser != null 
                ? $"{stockRequest.RequestedByUser.FirstName} {stockRequest.RequestedByUser.LastName}".Trim() 
                : string.Empty,
                
            DecidedByUserId = stockRequest.DecidedByUserId,
            DecidedByUserName = stockRequest.DecidedByUser != null 
                ? $"{stockRequest.DecidedByUser.FirstName} {stockRequest.DecidedByUser.LastName}".Trim() 
                : null,
            RejectionReason = stockRequest.RejectionReason,
            DecidedAt = stockRequest.DecidedAt
        };
    }
}