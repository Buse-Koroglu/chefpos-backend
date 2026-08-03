using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.StockRequests.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.StockRequests.Commands.ApproveStockRequest;

public class ApproveStockRequestCommandHandler : IRequestHandler<ApproveStockRequestCommand, StockRequestResponseDto>
{
    private readonly IStockRequestRepository _stockRequestRepository;
    private readonly IUserRepository _userRepository;

    public ApproveStockRequestCommandHandler(IStockRequestRepository stockRequestRepository, IUserRepository userRepository)
    {
        _stockRequestRepository = stockRequestRepository;
        _userRepository = userRepository;
    }

    public async Task<StockRequestResponseDto> Handle(ApproveStockRequestCommand request, CancellationToken cancellationToken)
    {
        var stockRequest = await _stockRequestRepository.GetByIdAsync(request.StockRequestId, cancellationToken)
            .OrThrowNotFoundAsync($"Stok talebi bulunamadı: {request.StockRequestId}");

        var decidedByUser = await _userRepository.GetByIdAsync(request.DecidedByUserId, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {request.DecidedByUserId}");

        if (decidedByUser.Role != Role.STOCK_MANAGER)
        {
            throw new InvalidOperationException("Sadece Yerleşke Stok Yetkilisi rolündeki kullanıcılar stok talebi onaylayabilir.");
        }

        if (!decidedByUser.HasAccessToLocation(stockRequest.LocationId))
        {
            throw new InvalidOperationException("Bu kullanıcının, stok talebinin ait olduğu yerleşkede yetkisi yok.");
        }

        stockRequest.Approve(request.DecidedByUserId);
        stockRequest.Ingredient.IncreaseStock(stockRequest.RequestedQuantity);

        await _stockRequestRepository.SaveAllChangesAsync(cancellationToken);

        return StockRequestResponseDto.FromEntity(stockRequest);
    }
}