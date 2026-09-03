using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Categories.DTOs;
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Categories.Commands.RemoveCategory;

public class DeactivateCategoryCommandHandler : IRequestHandler<DeactivateCategoryCommand, CategoryResponseDto>
{

    private readonly ICategoryRepository _categoryRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeactivateCategoryCommandHandler(ICategoryRepository categoryRepository, ILocationRepository locationRepository, IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _categoryRepository = categoryRepository;
        _locationRepository = locationRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<CategoryResponseDto> Handle(DeactivateCategoryCommand request,CancellationToken cancellationToken)
    {
        await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken).OrThrowNotFoundAsync($"Yerleşke Bulunamadı : {request.LocationId}");

        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken).OrThrowNotFoundAsync($"Kategori bulunamadı : {request.Id}");

        if (!category.BelongsToLocation(request.LocationId))
            throw new ForbiddenException("Bu işleme yetkiniz bulunmamaktır.");

        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        if (!actingUser.HasRole(Role.SUPER_ADMIN) && !actingUser.HasRoleAtLocation(Role.ADMIN, request.LocationId))
            throw new ForbiddenException("Bu işleme yetkiniz bulunmamaktır.");

        category.DeactivateCategory();
        await _categoryRepository.SaveAllChangesAsync(cancellationToken);

        return CategoryResponseDto.FromEntity(category);
    }
}