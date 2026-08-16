using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Users.DTOs;
using MediatR;
namespace ChefPos.Application.Users.Commands.RevokeLocationAccess;

public class RevokeLocationAccessCommandHandler : IRequestHandler<RevokeLocationAccessCommand, UserResponseDto>
{
    private readonly IUserRepository _userRepository;

    public RevokeLocationAccessCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponseDto> Handle(RevokeLocationAccessCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            throw new NotFoundException($"Kullanıcı bulunamadı: {request.UserId}");

        user.RevokeLocationAccess(request.LocationId);
        await _userRepository.SaveAllChangesAsync(cancellationToken);

        return UserResponseDto.FromEntity(user);
    }
}