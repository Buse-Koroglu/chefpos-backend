using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Menus.Commands.CreateProductForMenu;

public class CreateProductForMenuCommand : IRequest<ProductResponseDto>
{
    public Guid MenuId { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public string? Description { get; set; }

    public CreateProductForMenuCommand(Guid menuId, string name, decimal price, string? description)
    {
        MenuId = menuId;
        Name = name;
        Price = price;
        Description = description;
    }
}