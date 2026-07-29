using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Commands.ActivateProduct;

public class ActivateProductCommand : IRequest<ProductResponseDto>
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    
    public ActivateProductCommand(Guid id, Guid locationId)
    {
        Id = id;
        LocationId = locationId;
    }
    
}