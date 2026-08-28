using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Ingredients.Commands.RecordIngredientPurchase;
using ChefPos.Application.Ingredients.DTOs;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using MediatR;

public class RecordIngredientPurchaseCommandHandler : IRequestHandler<RecordIngredientPurchaseCommand, IngredientResponseDto>
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public RecordIngredientPurchaseCommandHandler(IIngredientRepository ingredientRepository, IStockMovementRepository stockMovementRepository, IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _ingredientRepository = ingredientRepository;
        _stockMovementRepository = stockMovementRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IngredientResponseDto> Handle(RecordIngredientPurchaseCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {currentUserId}");

        if (!currentUser.HasRole(Role.INVENTORY_STAFF) && !currentUser.HasRole(Role.STOCK_MANAGER) && !currentUser.HasRole(Role.ADMIN) && !currentUser.HasRole(Role.SUPER_ADMIN))
            throw new ValidationException("Sadece depo görevlisi, stok yöneticisi veya admin alış kaydı girebilir.");
        

        var ingredient = await _ingredientRepository.GetByIdAsync(request.IngredientId, cancellationToken).OrThrowNotFoundAsync($"Ham madde bulunamadı: {request.IngredientId}");

        if (!currentUser.HasRole(Role.SUPER_ADMIN) && !currentUser.HasAccessToLocation(ingredient.LocationId))
            throw new ValidationException("Bu kullanıcının, hammaddenin bulunduğu yerleşkeye erişimi yok.");

        var lot = ingredient.AddPurchaseLot(request.Quantity, request.UnitPrice);
        var movement = StockMovement.CreatePurchase(ingredient.Id, ingredient.LocationId, request.Quantity, currentUserId, lot, request.Note);

        await _stockMovementRepository.AddAsync(movement, cancellationToken);
        await _ingredientRepository.SaveAllChangesAsync(cancellationToken);

        return IngredientResponseDto.FromEntity(ingredient);
    }
}