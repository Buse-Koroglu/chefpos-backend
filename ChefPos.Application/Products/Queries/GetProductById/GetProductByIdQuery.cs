using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<ProductResponseDto>
{
    public Guid Id { get; set; }
    
    public GetProductByIdQuery(Guid id)
    {
        Id = id;
    }
    
}