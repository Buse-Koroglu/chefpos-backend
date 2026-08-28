using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.StockRequests.Commands.RejectStockRequest;
using ChefPos.Application.StockRequests.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.StockRequests.Commands.RejectStockRequest;

public class RejectStockRequestCommandHandler : IRequestHandler<RejectStockRequestCommand, StockRequestResponseDto>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IStockRequestRepository _stockRequestRepository;
    private readonly IUserRepository _userRepository;

    public RejectStockRequestCommandHandler(ICurrentUserService currentUserService, IStockRequestRepository stockRequestRepository,IUserRepository userRepository)
    {
        _currentUserService = currentUserService;
        _stockRequestRepository = stockRequestRepository;
        _userRepository = userRepository;
    }

    public async Task<StockRequestResponseDto> Handle(RejectStockRequestCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        var stockRequest = await _stockRequestRepository.GetByIdAsync(request.StockRequestId, cancellationToken).OrThrowNotFoundAsync($"Stok talebi bulunamadı: {request.StockRequestId}");

        var decidedByUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {currentUserId}");

        if (!decidedByUser.HasRole(Role.STOCK_MANAGER))
        {
            throw new ValidationException("Sadece Yerleşke Stok Yetkilisi rolündeki kullanıcılar stok talebi reddedebilir.");
        }

        if (!decidedByUser.HasAccessToLocation(stockRequest.LocationId))
        {
            throw new ValidationException("Bu kullanıcının, stok talebinin ait olduğu yerleşkede yetkisi yok.");
        }

        stockRequest.Reject(currentUserId, request.Reason);

        await _stockRequestRepository.SaveAllChangesAsync(cancellationToken);

        return StockRequestResponseDto.FromEntity(stockRequest);
    }
}