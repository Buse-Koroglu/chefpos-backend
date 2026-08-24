using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Products.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Products.Commands.AddProductIngredient;

public class AddProductIngredientCommandHandler : IRequestHandler<AddProductIngredientCommand, ProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public AddProductIngredientCommandHandler(
        IProductRepository productRepository,
        IIngredientRepository ingredientRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _productRepository = productRepository;
        _ingredientRepository = ingredientRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<ProductResponseDto> Handle(AddProductIngredientCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            throw new NotFoundException("Ürün bulunamadı.");
        }

        if (!product.BelongsToLocation(request.LocationId))
        {
            throw new ValidationException("Ürün bu yerleşkede tanımlı değil.");
        }

        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        if (!actingUser.HasRole(Role.SUPER_ADMIN) && !actingUser.HasAccessToLocation(request.LocationId))
        {
            throw new ValidationException("Bu yerleşke için işlem yapma yetkiniz yok.");
        }

        var ingredient = await _ingredientRepository.GetByIdAsync(request.IngredientId, cancellationToken);
        if (ingredient is null)
        {
            throw new NotFoundException("Ham madde bulunamadı.");
        }

        if (ingredient.LocationId != request.LocationId)
        {
            throw new ValidationException("Ham madde bu yerleşkeye ait değil.");
        }

        product.AddIngredient(request.LocationId, request.IngredientId, request.QuantityPerServing);

        await _productRepository.SaveAllChangesAsync(cancellationToken);

        return ProductResponseDto.FromEntity(product);
    }
}