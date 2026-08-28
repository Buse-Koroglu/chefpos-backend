using ChefPos.Application.Common.Models;

namespace ChefPos.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveImageAsync(FileUploadRequest file, string subfolder, CancellationToken cancellationToken);
    
    Task DeleteAsync(string? publicPath, CancellationToken cancellationToken);
}
