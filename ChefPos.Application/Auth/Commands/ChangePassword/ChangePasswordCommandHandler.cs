using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using MediatR;

namespace ChefPos.Application.Auth.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserService _currentUserService;

    public ChangePasswordCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _currentUserService = currentUserService;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        var user = await _userRepository.GetByIdAsync(currentUserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {currentUserId}");
        var hashedPassword = _passwordHasher.Hash(request.NewPassword);
        user.ChangePassword(hashedPassword);
        await _userRepository.SaveAllChangesAsync(cancellationToken);
    }
}