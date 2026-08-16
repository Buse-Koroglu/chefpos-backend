using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Users.DTOs;
using MediatR;

namespace ChefPos.Application.Users.Commands.ActivateUser;

public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, UserResponseDto>
{
    private readonly IUserRepository _userRepository;

    public ActivateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponseDto> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            throw new NotFoundException($"Kullanıcı bulunamadı: {request.UserId}");

        user.ActivateUser();
        await _userRepository.SaveAllChangesAsync(cancellationToken);

        return UserResponseDto.FromEntity(user);
    }
}