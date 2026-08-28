using ChefPos.Domain.Entities;

namespace ChefPos.Application.Locations.DTOs;

public class LocationResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public bool IsActive { get; set; }
    public int EmployeeCount { get; set; }


    public static LocationResponseDto FromEntity(Location location,int employeeCount = 0)
    {
        return new LocationResponseDto
        {
            Id = location.Id,
            Name = location.Name,
            IsActive = location.IsActive,
            EmployeeCount = employeeCount
        };
    }
}