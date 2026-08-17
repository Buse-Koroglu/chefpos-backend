using ChefPos.Application.Categories.DTOs;
using MediatR;

namespace ChefPos.Application.Categories.Commands.AddCategoryLocation;

public class AddCategoryLocationCommand : IRequest<CategoryResponseDto>
{
    public Guid CategoryId { get; }
    public Guid LocationId { get; }

    public AddCategoryLocationCommand(Guid categoryId, Guid locationId)
    {
        CategoryId = categoryId;
        LocationId = locationId;
    }
}
