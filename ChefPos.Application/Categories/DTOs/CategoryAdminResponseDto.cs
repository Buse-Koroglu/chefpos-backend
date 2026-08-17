namespace ChefPos.Application.Categories.DTOs;

public class CategoryAdminResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Icon { get; set; }
    public bool IsActive { get; set; }
    public Guid LocationId { get; set; }
    public string LocationName { get; set; } = default!;
    public int ProductCount { get; set; }
}