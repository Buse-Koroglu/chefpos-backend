using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Tables.DTOs;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Tables.Commands.CreateTable;

public class CreateTableCommandHandler : IRequestHandler<CreateTableCommand, TableResponseDto>
{
    private readonly ITableRepository _tableRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateTableCommandHandler(ITableRepository tableRepository,ILocationRepository locationRepository, IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _tableRepository = tableRepository;
        _locationRepository = locationRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<TableResponseDto> Handle(CreateTableCommand request, CancellationToken cancellationToken)
    {
        await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken).OrThrowNotFoundAsync($"Yerleşke bulunamadı: {request.LocationId}");

        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        if (!actingUser.HasRole(Role.SUPER_ADMIN) && !actingUser.HasRoleAtLocation(Role.ADMIN, request.LocationId))
        {
            throw new ValidationException("Sadece kendi yerleşkeniz için masa oluşturabilirsiniz.");
        }

        if (await _tableRepository.ExistsByNumberAsync(request.LocationId, request.TableNumber, null, cancellationToken))
        {
            throw new ValidationException($"Bu yerleşkede {request.TableNumber} numaralı masa zaten mevcut.");
        }

        var table = new Table(request.LocationId, request.TableNumber);

        await _tableRepository.AddAsync(table, cancellationToken);
        await _tableRepository.SaveAllChangesAsync(cancellationToken);

        return TableResponseDto.FromEntity(table);
    }
}
