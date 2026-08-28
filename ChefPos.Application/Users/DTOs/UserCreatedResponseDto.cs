using ChefPos.Domain.Entities;

namespace ChefPos.Application.Users.DTOs;

public class UserCreatedResponseDto : UserResponseDto
{
    public string GeneratedPassword { get; set; } = default!;

    public static UserCreatedResponseDto FromEntity(User user, string generatedPassword)
    {
        var baseDto = FromEntity(user);

        return new UserCreatedResponseDto
        {
            Id = baseDto.Id,
            PersonalId = baseDto.PersonalId,
            FirstName = baseDto.FirstName,
            LastName = baseDto.LastName,
            Roles = baseDto.Roles,
            IsFirstLogin = baseDto.IsFirstLogin,
            IsActive = baseDto.IsActive,
            LocationIds = baseDto.LocationIds,
            GeneratedPassword = generatedPassword
        };
    }
}
