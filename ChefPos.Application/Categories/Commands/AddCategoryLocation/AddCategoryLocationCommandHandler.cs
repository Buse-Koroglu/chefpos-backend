using ChefPos.Application.Categories.DTOs;
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Categories.Commands.AddCategoryLocation;

public class AddCategoryLocationCommandHandler : IRequestHandler<AddCategoryLocationCommand, CategoryResponseDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public AddCategoryLocationCommandHandler(ICategoryRepository categoryRepository, ILocationRepository locationRepository, IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _categoryRepository = categoryRepository;
        _locationRepository = locationRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<CategoryResponseDto> Handle(AddCategoryLocationCommand request, CancellationToken cancellationToken)
    {
        await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken).OrThrowNotFoundAsync($"Yerleşke bulunamadı: {request.LocationId}");

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken).OrThrowNotFoundAsync($"Kategori bulunamadı: {request.CategoryId}");

        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        if (!actingUser.HasRole(Role.SUPER_ADMIN) && !actingUser.HasAccessToLocation(request.LocationId))
            throw new ValidationException("Bu yerleşke için işlem yapma yetkiniz yok.");

        category.AddLocation(request.LocationId);
        await _categoryRepository.SaveAllChangesAsync(cancellationToken);

        return CategoryResponseDto.FromEntity(category);
    }
}
