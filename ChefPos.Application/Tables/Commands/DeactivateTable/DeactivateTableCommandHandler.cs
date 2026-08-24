using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Tables.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Tables.Commands.DeactivateTable;

public class DeactivateTableCommandHandler : IRequestHandler<DeactivateTableCommand, TableResponseDto>
{
    private readonly ITableRepository _tableRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeactivateTableCommandHandler(ITableRepository tableRepository, IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _tableRepository = tableRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<TableResponseDto> Handle(DeactivateTableCommand request, CancellationToken cancellationToken)
    {
        var table = await _tableRepository.GetByIdAsync(request.TableId, cancellationToken)
            .OrThrowNotFoundAsync($"Masa bulunamadı: {request.TableId}");

        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        if (!actingUser.HasRole(Role.SUPER_ADMIN) && !actingUser.HasAccessToLocation(table.LocationId))
        {
            throw new ValidationException("Bu masayı yönetme yetkiniz yok.");
        }

        table.Deactivate();

        await _tableRepository.SaveAllChangesAsync(cancellationToken);

        return TableResponseDto.FromEntity(table);
    }
}
