namespace ChefPos.Application.Products.DTOs;

public class ProductAdminResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public List<Guid> LocationIds { get; set; } = new();
    public List<string> LocationNames { get; set; } = new();
}
