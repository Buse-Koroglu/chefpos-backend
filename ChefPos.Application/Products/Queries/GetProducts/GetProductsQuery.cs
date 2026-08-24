using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Queries.GetProducts;

public class GetProductsQuery : IRequest<List<ProductResponseDto>>
{
    public Guid LocationId { get; set; }
    public Guid? CategoryId { get; set; }
    public bool IncludeInactive { get; set; }
    public bool IncludeUncategorized { get; set; }

    public GetProductsQuery(Guid locationId, Guid? categoryId, bool includeInactive, bool includeUncategorized = false)
    {
        LocationId = locationId;
        CategoryId = categoryId;
        IncludeInactive = includeInactive;
        IncludeUncategorized = includeUncategorized;
    }
}