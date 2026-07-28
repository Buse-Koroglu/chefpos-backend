using ChefPos.Application.Categories.DTOs;
using MediatR;

namespace ChefPos.Application.Categories.Commands.ActivateCategory;

public class ActivateCategoryCommand : IRequest<CategoryResponseDto>
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    
    
}