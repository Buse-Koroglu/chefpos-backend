using ChefPos.Application.Common.Models;

namespace ChefPos.Application.Common.Interfaces;

public interface IFileStorageService
{
    /// <summary>
    /// Validates, optimizes and persists an uploaded image under the given subfolder.
    /// Returns the public, servable path of the stored file (e.g. "/uploads/products/{guid}.webp").
    /// </summary>
    Task<string> SaveImageAsync(FileUploadRequest file, string subfolder, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a previously stored file given its public path. Safe to call with null/unknown paths (no-op).
    /// </summary>
    Task DeleteAsync(string? publicPath, CancellationToken cancellationToken);
}
