using ChefPos.Domain.Entities;

namespace ChefPos.Application.Menus.DTOs;

public class MenuResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public Guid LocationId { get; set; }
    public bool IsActive { get; set; }
    public List<MenuProductDto> Products { get; set; } = new();

    public static MenuResponseDto FromEntity(Menu menu) => new MenuResponseDto
    {
        Id = menu.Id,
        Name = menu.Name,
        Description = menu.Description,
        LocationId = menu.LocationId,
        IsActive = menu.IsActive,
        Products = menu.MenuProducts.OrderBy(mp => mp.DisplayOrder).Select(mp => new MenuProductDto
        {
            ProductId = mp.ProductId,
            ProductName = mp.Product.Name,
            Price = mp.Product.Price,
            ImageUrl = mp.Product.ImageUrl,
            ProductIsActive = mp.Product.IsActive,
        }).ToList(),
    };
}

public class MenuProductDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool ProductIsActive { get; set; }
}