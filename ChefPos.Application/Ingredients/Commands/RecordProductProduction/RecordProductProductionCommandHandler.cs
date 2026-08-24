using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Ingredients.Commands.RecordProductProduction;
using ChefPos.Application.Ingredients.DTOs;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using MediatR;

public class RecordProductProductionCommandHandler : IRequestHandler<RecordProductProductionCommand, List<IngredientResponseDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public RecordProductProductionCommandHandler(
        IProductRepository productRepository,
        IStockMovementRepository stockMovementRepository,
        IIngredientRepository ingredientRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _productRepository = productRepository;
        _stockMovementRepository = stockMovementRepository;
        _ingredientRepository = ingredientRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<IngredientResponseDto>> Handle(RecordProductProductionCommand request, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
        {
            throw new ValidationException("Üretilen adet 0'dan büyük olmalı.");
        }

        var currentUserId = _currentUserService.UserId;
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);
        if (currentUser is null)
        {
            throw new NotFoundException($"Kullanıcı bulunamadı: {currentUserId}");
        }

        if (!currentUser.HasRole(Role.INVENTORY_STAFF) && !currentUser.HasRole(Role.STOCK_MANAGER) && !currentUser.HasRole(Role.ADMIN) && !currentUser.HasRole(Role.SUPER_ADMIN))
        {
            throw new ValidationException("Sadece depo görevlisi, stok yöneticisi veya admin üretim kaydı girebilir.");
        }

        if (!currentUser.HasRole(Role.SUPER_ADMIN) && !currentUser.HasAccessToLocation(request.LocationId))
        {
            throw new ValidationException("Bu kullanıcının belirtilen yerleşkeye erişimi yok.");
        }

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            throw new NotFoundException($"Ürün bulunamadı: {request.ProductId}");
        }

        var productLocation = product.ProductLocations.FirstOrDefault(pl => pl.LocationId == request.LocationId);
        if (productLocation is null)
        {
            throw new ValidationException($"'{product.Name}' ürünü bu yerleşkede tanımlı değil.");
        }

        if (!productLocation.ProductItems.Any())
        {
            throw new ValidationException($"'{product.Name}' ürününün bu yerleşkede tanımlı bir reçetesi (ham madde listesi) yok.");
        }

        var affectedIngredients = new List<Ingredient>();

        foreach (var recipeLine in productLocation.ProductItems)
        {
            var amountToDeduct = recipeLine.QuantityPerServing * request.Quantity;
            var ingredient = recipeLine.Ingredient;

            var consumptions = ingredient.DeductStockFifo(amountToDeduct);
            var movement = StockMovement.CreateProductionDeduction(
                ingredient.Id, ingredient.LocationId, currentUserId, product.Id, consumptions,
                request.Note ?? $"'{product.Name}' üretimi ({request.Quantity} adet)");

            await _stockMovementRepository.AddAsync(movement, cancellationToken);
            affectedIngredients.Add(ingredient);
        }

        await _ingredientRepository.SaveAllChangesAsync(cancellationToken);

        return affectedIngredients.Select(IngredientResponseDto.FromEntity).ToList();
    }
}