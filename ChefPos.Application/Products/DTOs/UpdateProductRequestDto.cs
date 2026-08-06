namespace ChefPos.Application.Products.DTOs;

public class UpdateProductRequestDto
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}