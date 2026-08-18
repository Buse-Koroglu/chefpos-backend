using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Common.Pagination;
using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Queries.GetProductsPaged;

public class GetProductsPagedQueryHandler : IRequestHandler<GetProductsPagedQuery, PagedResult<ProductAdminResponseDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsPagedQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResult<ProductAdminResponseDto>> Handle(GetProductsPagedQuery request, CancellationToken cancellationToken)
    {
        var (products, totalCount) = await _productRepository.GetAllPagedAsync(
            request.SearchTerm, request.LocationId, request.CategoryId, request.IsActive, request.PageNumber, request.PageSize, cancellationToken);

        var items = products.Select(p => new ProductAdminResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            IsActive = p.IsActive,
            CategoryId = p.CategoryId,
            CategoryName = p.Category.Name,
            LocationIds = p.ProductLocations.Select(pl => pl.LocationId).ToList(),
            LocationNames = p.ProductLocations.Select(pl => pl.Location.Name).ToList()
        }).ToList();

        return new PagedResult<ProductAdminResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
