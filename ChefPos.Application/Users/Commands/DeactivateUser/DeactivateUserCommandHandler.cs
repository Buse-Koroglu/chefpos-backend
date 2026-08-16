using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Users.Commands.DeactivateUser;
using ChefPos.Application.Users.DTOs;
using MediatR;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, UserResponseDto>
{
    private readonly IUserRepository _userRepository;

    public DeactivateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponseDto> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            throw new NotFoundException($"Kullanıcı bulunamadı: {request.UserId}");

        user.DeactivateUser();
        await _userRepository.SaveAllChangesAsync(cancellationToken);

        return UserResponseDto.FromEntity(user);
    }
}