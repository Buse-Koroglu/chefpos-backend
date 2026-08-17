using ChefPos.Application.Categories.DTOs;
using MediatR;

namespace ChefPos.Application.Categories.Commands.RemoveCategoryLocation;

public class RemoveCategoryLocationCommand : IRequest<CategoryResponseDto>
{
    public Guid CategoryId { get; }
    public Guid LocationId { get; }

    public RemoveCategoryLocationCommand(Guid categoryId, Guid locationId)
    {
        CategoryId = categoryId;
        LocationId = locationId;
    }
}
