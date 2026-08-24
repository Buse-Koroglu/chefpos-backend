using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Users.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetUserByIdQueryHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<UserResponseDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {request.Id}");

        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        var isSuperAdmin = actingUser.HasRole(Role.SUPER_ADMIN);
        var actingUserLocationIds = actingUser.Locations.Select(l => l.LocationId).ToHashSet();

        if (!isSuperAdmin && !user.Locations.Select(l => l.LocationId).Any(id => actingUserLocationIds.Contains(id)))
        {
            throw new ValidationException("Bu kullanıcıyı görüntüleme yetkiniz yok.");
        }

        var dto = UserResponseDto.FromEntity(user);
        if (!isSuperAdmin)
        {
            dto.LocationIds = dto.LocationIds.Where(id => actingUserLocationIds.Contains(id)).ToList();
        }

        return dto;
    }
}