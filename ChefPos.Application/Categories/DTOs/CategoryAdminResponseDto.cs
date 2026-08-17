namespace ChefPos.Application.Categories.DTOs;

public class CategoryAdminResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Icon { get; set; }
    public bool IsActive { get; set; }
    public List<Guid> LocationIds { get; set; } = new();
    public List<string> LocationNames { get; set; } = new();
    public int ProductCount { get; set; }
}