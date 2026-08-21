using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Users.DTOs;
using MediatR;

namespace ChefPos.Application.Users.Queries.GetStockManagerByLocation;

public class GetStockManagerByLocationQueryHandler : IRequestHandler<GetStockManagerByLocationQuery, UserResponseDto?>
{
    private readonly IUserRepository _userRepository;

    public GetStockManagerByLocationQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponseDto?> Handle(GetStockManagerByLocationQuery request, CancellationToken cancellationToken)
    {
        var stockManager = await _userRepository.GetStockManagerByLocationAsync(request.LocationId, cancellationToken);

        return stockManager is null ? null : UserResponseDto.FromEntity(stockManager);
    }
}
