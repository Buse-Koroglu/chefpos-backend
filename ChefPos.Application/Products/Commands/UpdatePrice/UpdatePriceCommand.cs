using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Commands.UpdatePrice;

public class UpdatePriceCommand : IRequest<ProductResponseDto>
{
    public Guid Id { get; set; }
    public decimal NewPrice { get; set; }

    public UpdatePriceCommand(Guid id, decimal price)
    {
        Id = id;
        NewPrice = price;
    }
}