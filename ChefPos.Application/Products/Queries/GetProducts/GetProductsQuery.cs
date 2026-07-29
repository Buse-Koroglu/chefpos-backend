using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Queries.GetProducts;

public class GetProductsQuery : IRequest<List<ProductResponseDto>>
{
    public Guid LocationId { get; set; }
    public Guid? CategoryId { get; set; }
    public bool IncludeInactive { get; set; }
 
    public GetProductsQuery(Guid locationId, Guid? categoryId, bool includeInactive)
    {
        LocationId = locationId;
        CategoryId = categoryId;
        IncludeInactive = includeInactive;
    }
}