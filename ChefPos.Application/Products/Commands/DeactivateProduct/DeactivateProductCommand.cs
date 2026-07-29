using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Commands.DeactivateProduct;

public class DeactivateProductCommand : IRequest<ProductResponseDto>
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }

    public DeactivateProductCommand(Guid id, Guid locationId)
    {
        Id = id;
        LocationId = locationId;
    }
    
}