using ChefPos.Application.Categories.DTOs;
using MediatR;

namespace ChefPos.Application.Categories.Commands.RemoveCategory;

public class DeactivateCategoryCommand : IRequest<CategoryResponseDto>
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    
    public DeactivateCategoryCommand(Guid id, Guid locationId)
    {
        Id = id;
        LocationId = locationId;
    }
}