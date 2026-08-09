using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.StockRequests.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.StockRequests.Commands.ApproveStockRequest;

public class ApproveStockRequestCommandHandler : IRequestHandler<ApproveStockRequestCommand, StockRequestResponseDto>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IStockRequestRepository _stockRequestRepository;
    private readonly IUserRepository _userRepository;

    public ApproveStockRequestCommandHandler(
        ICurrentUserService currentUserService,
        IStockRequestRepository stockRequestRepository,
        IUserRepository userRepository)
    {
        _currentUserService = currentUserService;
        _stockRequestRepository = stockRequestRepository;
        _userRepository = userRepository;
    }

    public async Task<StockRequestResponseDto> Handle(ApproveStockRequestCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        var stockRequest = await _stockRequestRepository.GetByIdAsync(request.StockRequestId, cancellationToken)
            .OrThrowNotFoundAsync($"Stok talebi bulunamadı: {request.StockRequestId}");

        var decidedByUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {currentUserId}");

        if (!decidedByUser.HasRole(Role.STOCK_MANAGER))
        {
            throw new ValidationException("Sadece Yerleşke Stok Yetkilisi rolündeki kullanıcılar stok talebi onaylayabilir.");
        }

        if (!decidedByUser.HasAccessToLocation(stockRequest.LocationId))
        {
            throw new ValidationException("Bu kullanıcının, stok talebinin ait olduğu yerleşkede yetkisi yok.");
        }

        stockRequest.Approve(currentUserId);
        stockRequest.Ingredient.IncreaseStock(stockRequest.RequestedQuantity);

        await _stockRequestRepository.SaveAllChangesAsync(cancellationToken);

        return StockRequestResponseDto.FromEntity(stockRequest);
    }
}