namespace ChefPos.Application.Menus.DTOs;

public class UpdateMenuRequestDto
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}