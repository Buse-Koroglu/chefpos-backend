using ChefPos.Application.Categories.DTOs;
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Categories.Commands.RemoveCategoryLocation;

public class RemoveCategoryLocationCommandHandler : IRequestHandler<RemoveCategoryLocationCommand, CategoryResponseDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public RemoveCategoryLocationCommandHandler(ICategoryRepository categoryRepository, IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<CategoryResponseDto> Handle(RemoveCategoryLocationCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken).OrThrowNotFoundAsync($"Kategori bulunamadı: {request.CategoryId}");

        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        if (!actingUser.HasRole(Role.SUPER_ADMIN) && !actingUser.HasRoleAtLocation(Role.ADMIN, request.LocationId))
            throw new ValidationException("Bu yerleşke için işlem yapma yetkiniz yok.");

        category.RemoveLocation(request.LocationId);
        await _categoryRepository.SaveAllChangesAsync(cancellationToken);

        return CategoryResponseDto.FromEntity(category);
    }
}
