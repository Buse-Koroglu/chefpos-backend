using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Common.Pagination;
using ChefPos.Application.Locations.DTOs;
using MediatR;

namespace ChefPos.Application.Locations.Queries.GetLocationsPaged;


public class GetLocationsPagedQueryHandler : IRequestHandler<GetLocationsPagedQuery, PagedResult<LocationResponseDto>>
{
    private readonly ILocationRepository _locationRepository;
    private readonly IUserRepository _userRepository;

    public GetLocationsPagedQueryHandler(ILocationRepository locationRepository, IUserRepository userRepository)
    {
        _locationRepository = locationRepository;
        _userRepository = userRepository;
    }

    public async Task<PagedResult<LocationResponseDto>> Handle(GetLocationsPagedQuery request, CancellationToken cancellationToken)
    {
        var (locations, totalCount) = await _locationRepository.GetAllPagedAsync(request.SearchTerm, request.IsActive, request.PageNumber, request.PageSize, cancellationToken);

        var employeeCounts = await _userRepository.GetEmployeeCountsByLocationAsync(cancellationToken);

        var items = locations.Select(location =>
            {
                var count = employeeCounts.FirstOrDefault(x => x.LocationId == location.Id).EmployeeCount;
                return LocationResponseDto.FromEntity(location, count);
            })
            .ToList();

        return new PagedResult<LocationResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}