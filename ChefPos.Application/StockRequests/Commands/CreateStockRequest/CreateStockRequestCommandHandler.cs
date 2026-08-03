using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.StockRequests.DTOs;
using ChefPos.Domain.Entities;
using MediatR;

namespace ChefPos.Application.StockRequests.Commands.CreateStockRequest;

public class CreateStockRequestCommandHandler : IRequestHandler<CreateStockRequestCommand, StockRequestResponseDto>
{
    private readonly IStockRequestRepository _stockRequestRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IUserRepository _userRepository;

    public CreateStockRequestCommandHandler(
        IStockRequestRepository stockRequestRepository,
        IIngredientRepository ingredientRepository,
        IUserRepository userRepository)
    {
        _stockRequestRepository = stockRequestRepository;
        _ingredientRepository = ingredientRepository;
        _userRepository = userRepository;
    }

    public async Task<StockRequestResponseDto> Handle(CreateStockRequestCommand request, CancellationToken cancellationToken)
    {
        var ingredient = await _ingredientRepository.GetByIdAsync(request.IngredientId, cancellationToken)
            .OrThrowNotFoundAsync($"Ham madde bulunamadı: {request.IngredientId}");

        var requestedByUser = await _userRepository.GetByIdAsync(request.RequestedByUserId, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {request.RequestedByUserId}");

        if (!requestedByUser.HasAccessToLocation(ingredient.LocationId))
        {
            throw new InvalidOperationException("Bu kullanıcının, hammaddenin bulunduğu yerleşkeye erişimi yok.");
        }

        if (await _stockRequestRepository.HasPendingAsync(request.IngredientId, ingredient.LocationId, cancellationToken))
        {
            throw new InvalidOperationException("Bu hammadde için bu lokasyonda zaten bekleyen bir stok talebi var.");
        }

        var stockRequest = new StockRequest(request.IngredientId, ingredient.LocationId, request.RequestedByUserId, request.RequestedQuantity);

        await _stockRequestRepository.AddAsync(stockRequest, cancellationToken);
        await _stockRequestRepository.SaveAllChangesAsync(cancellationToken);

        return StockRequestResponseDto.FromEntity(stockRequest);
    }
}