using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.StockRequests.DTOs;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.StockRequests.Commands.CreateStockRequest;

public class CreateStockRequestCommandHandler : IRequestHandler<CreateStockRequestCommand, StockRequestResponseDto>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IStockRequestRepository _stockRequestRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IUserRepository _userRepository;

    public CreateStockRequestCommandHandler(
        ICurrentUserService currentUserService,
        IStockRequestRepository stockRequestRepository,
        IIngredientRepository ingredientRepository,
        IUserRepository userRepository)
    {
        _currentUserService = currentUserService;
        _stockRequestRepository = stockRequestRepository;
        _ingredientRepository = ingredientRepository;
        _userRepository = userRepository;
    }

    public async Task<StockRequestResponseDto> Handle(CreateStockRequestCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        var ingredient = await _ingredientRepository.GetByIdAsync(request.IngredientId, cancellationToken)
            .OrThrowNotFoundAsync($"Ham madde bulunamadı: {request.IngredientId}");

        var requestedByUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {currentUserId}");

        if (!requestedByUser.HasRole(Role.INVENTORY_STAFF))
        {
            throw new ValidationException("Sadece envanter (INVENTORY_STAFF) personeli stok talebi oluşturabilir.");
        }

        if (!requestedByUser.HasAccessToLocation(ingredient.LocationId))
        {
            throw new ValidationException("Bu kullanıcının, hammaddenin bulunduğu yerleşkeye erişimi yok.");
        }

        if (await _stockRequestRepository.HasPendingAsync(request.IngredientId, ingredient.LocationId, cancellationToken))
        {
            throw new ValidationException("Bu hammadde için bu lokasyonda zaten bekleyen bir stok talebi var.");
        }

        var stockRequest = new StockRequest(request.IngredientId, ingredient.LocationId, currentUserId, request.RequestedQuantity);

        await _stockRequestRepository.AddAsync(stockRequest, cancellationToken);
        await _stockRequestRepository.SaveAllChangesAsync(cancellationToken);

        return StockRequestResponseDto.FromEntity(stockRequest);
    }
}