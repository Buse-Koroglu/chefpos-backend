namespace ChefPos.Application.Menus.DTOs;

public class CreateProductForMenuRequestDto
{
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
}