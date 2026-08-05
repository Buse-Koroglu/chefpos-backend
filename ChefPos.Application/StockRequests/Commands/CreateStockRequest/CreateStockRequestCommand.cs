using ChefPos.Application.StockRequests.DTOs;
using MediatR;

namespace ChefPos.Application.StockRequests.Commands.CreateStockRequest;

public class CreateStockRequestCommand : IRequest<StockRequestResponseDto>
{
    public Guid IngredientId { get; set; }
    public decimal RequestedQuantity { get; set; }
}
