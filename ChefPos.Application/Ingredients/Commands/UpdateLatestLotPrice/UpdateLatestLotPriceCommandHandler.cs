using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Ingredients.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Ingredients.Commands.UpdateIngredientPrice;

public class UpdateLatestLotPriceCommandHandler : IRequestHandler<UpdateLatestLotPriceCommand, IngredientResponseDto>
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateLatestLotPriceCommandHandler(IIngredientRepository ingredientRepository, IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _ingredientRepository = ingredientRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IngredientResponseDto> Handle(UpdateLatestLotPriceCommand request, CancellationToken cancellationToken)
    {
        var ingredient = await _ingredientRepository.GetByIdAsync(request.IngredientId, cancellationToken).OrThrowNotFoundAsync($"Ham madde bulunamadı: {request.IngredientId}");

        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        if (!actingUser.HasRole(Role.SUPER_ADMIN) && !actingUser.HasRoleAtLocation(Role.ADMIN, ingredient.LocationId))
        {
            throw new ValidationException("Bu ham maddeyi yönetme yetkiniz yok.");
        }

        ingredient.UpdateLatestPurchasePrice(request.UnitPrice);
        await _ingredientRepository.SaveAllChangesAsync(cancellationToken);
        return IngredientResponseDto.FromEntity(ingredient);
    }
}
