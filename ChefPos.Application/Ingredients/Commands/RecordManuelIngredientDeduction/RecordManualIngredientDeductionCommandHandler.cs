using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Ingredients.DTOs;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Ingredients.Commands.RecordManuelIngredientDeduction;

public class RecordManualIngredientDeductionCommandHandler : IRequestHandler<RecordManualIngredientDeductionCommand, IngredientResponseDto>
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public RecordManualIngredientDeductionCommandHandler(
        IIngredientRepository ingredientRepository,
        IStockMovementRepository stockMovementRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _ingredientRepository = ingredientRepository;
        _stockMovementRepository = stockMovementRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IngredientResponseDto> Handle(RecordManualIngredientDeductionCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Note))
        {
            throw new ValidationException("Elden düşme için bir açıklama/gerekçe girilmelidir.");
        }

        var currentUserId = _currentUserService.UserId;
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {currentUserId}");

        if (!currentUser.HasRole(Role.INVENTORY_STAFF) && !currentUser.HasRole(Role.STOCK_MANAGER) && !currentUser.HasRole(Role.ADMIN) && !currentUser.HasRole(Role.SUPER_ADMIN))
        {
            throw new ValidationException("Sadece depo görevlisi, stok yöneticisi veya admin elden stok düşümü yapabilir.");
        }

        var ingredient = await _ingredientRepository.GetByIdAsync(request.IngredientId, cancellationToken)
            .OrThrowNotFoundAsync($"Ham madde bulunamadı: {request.IngredientId}");

        if (!currentUser.HasRole(Role.SUPER_ADMIN) && !currentUser.HasAccessToLocation(ingredient.LocationId))
        {
            throw new ValidationException("Bu kullanıcının, hammaddenin bulunduğu yerleşkeye erişimi yok.");
        }

        var consumptions = ingredient.DeductStockFifo(request.Quantity);
        var movement = StockMovement.CreateManualDeduction(ingredient.Id, ingredient.LocationId, currentUserId, consumptions, request.Note);

        await _stockMovementRepository.AddAsync(movement, cancellationToken);
        await _ingredientRepository.SaveAllChangesAsync(cancellationToken);

        return IngredientResponseDto.FromEntity(ingredient);
    }
}