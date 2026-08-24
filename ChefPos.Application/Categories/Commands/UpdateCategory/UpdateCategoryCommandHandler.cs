using ChefPos.Application.Categories.DTOs;
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand,CategoryResponseDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<CategoryResponseDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken).OrThrowNotFoundAsync($"Kategori Bulunamadı: {request.CategoryId}");

        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        if (!actingUser.HasRole(Role.SUPER_ADMIN) && !category.CategoryLocations.Any(cl => actingUser.HasAccessToLocation(cl.LocationId)))
        {
            throw new ValidationException("Bu kategoriyi yönetme yetkiniz yok.");
        }

        category.UpdateDetails(request.Name,request.Icon);
        await _categoryRepository.SaveAllChangesAsync(cancellationToken);
        return CategoryResponseDto.FromEntity(category);
    }
}