using ChefPos.Domain.Entities;

namespace ChefPos.Application.Categories.DTOs;

public class CategoryResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Icon { get; set; }
    public bool IsActive { get; set; }
    public Guid LocationId { get; set; }

    public static CategoryResponseDto FromEntity(Category category)
    {
        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Icon = category.Icon,
            IsActive = category.IsActive,
            LocationId = category.LocationId
        };
    }
}