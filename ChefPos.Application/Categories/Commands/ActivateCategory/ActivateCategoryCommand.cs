using ChefPos.Application.Categories.DTOs;
using MediatR;

public class ActivateCategoryCommand : IRequest<CategoryResponseDto>
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }

    public ActivateCategoryCommand(Guid id, Guid locationId)
    {
        Id = id;
        LocationId = locationId;
    }
}