using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Users.Commands.AddRole;
using ChefPos.Application.Users.DTOs;
using MediatR;

public class AddRoleCommandHandler : IRequestHandler<AddRoleCommand, UserResponseDto>
{
    private readonly IUserRepository _userRepository;

    public AddRoleCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponseDto> Handle(AddRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {request.UserId}");

        user.AddRole(request.Role);

        await _userRepository.SaveAllChangesAsync(cancellationToken);

        return UserResponseDto.FromEntity(user);
    }
}