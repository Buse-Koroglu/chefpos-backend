using ChefPos.Application.Users.DTOs;
using MediatR;

namespace ChefPos.Application.Users.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<UserResponseDto>{
    public Guid Id { get; set; }
    public GetUserByIdQuery(Guid id)
    {
        Id = id;
    }
}