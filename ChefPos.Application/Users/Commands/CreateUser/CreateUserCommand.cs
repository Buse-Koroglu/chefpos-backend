using ChefPos.Application.Users.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Users.Commands.CreateUser;

public class CreateUserCommand : IRequest<UserResponseDto>
{
    public string PersonalId { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public List<Role> Roles { get; set; } = new();
}