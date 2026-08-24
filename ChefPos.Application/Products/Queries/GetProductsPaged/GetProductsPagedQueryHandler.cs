using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Common.Pagination;
using ChefPos.Application.Products.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Products.Queries.GetProductsPaged;

public class GetProductsPagedQueryHandler : IRequestHandler<GetProductsPagedQuery, PagedResult<ProductAdminResponseDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetProductsPagedQueryHandler(IProductRepository productRepository, IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _productRepository = productRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<ProductAdminResponseDto>> Handle(GetProductsPagedQuery request, CancellationToken cancellationToken)
    {
        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        var locationId = request.LocationId;
        if (!actingUser.HasRole(Role.SUPER_ADMIN))
        {
            if (locationId.HasValue)
            {
                if (!actingUser.HasAccessToLocation(locationId.Value))
                    throw new ValidationException("Bu yerleşke için işlem yapma yetkiniz yok.");
            }
            else
            {
                locationId = actingUser.Locations.Select(l => l.LocationId).FirstOrDefault();
            }
        }

        var (products, totalCount) = await _productRepository.GetAllPagedAsync(
            request.SearchTerm, locationId, request.CategoryId, request.IsActive, request.PageNumber, request.PageSize, request.IncludeUncategorized, cancellationToken);

        var isSuperAdmin = actingUser.HasRole(Role.SUPER_ADMIN);

        var items = products.Select(p =>
        {
            var visibleLocations = isSuperAdmin
                ? p.ProductLocations
                : p.ProductLocations.Where(pl => pl.LocationId == locationId);

            return new ProductAdminResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                IsActive = p.IsActive,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                LocationIds = visibleLocations.Select(pl => pl.LocationId).ToList(),
                LocationNames = visibleLocations.Select(pl => pl.Location.Name).ToList()
            };
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
