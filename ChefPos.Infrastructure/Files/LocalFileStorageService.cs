using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Common.Models;
using ChefPos.Application.Common.Settings;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace ChefPos.Infrastructure.Files;

public class LocalFileStorageService : IFileStorageService
{
    private const int MaxPixelLimit = 1600;

    private readonly FileStorageSettings _settings;

    public LocalFileStorageService(IOptions<FileStorageSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<string> SaveImageAsync(FileUploadRequest file, string subfolder, CancellationToken cancellationToken)
    {
        using var image = await Image.LoadAsync(file.Content, cancellationToken);

        if (image.Width > MaxPixelLimit || image.Height > MaxPixelLimit)
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(MaxPixelLimit, MaxPixelLimit)
            }));
        }

        var fileName = $"{Guid.NewGuid():N}.webp";
        var directory = Path.Combine(_settings.RootPath, subfolder);
        Directory.CreateDirectory(directory);
        var physicalPath = Path.Combine(directory, fileName);

        await image.SaveAsync(physicalPath, new WebpEncoder { Quality = 80 }, cancellationToken); // saved as webp all all images

        return $"{_settings.PublicBasePath.TrimEnd('/')}/{subfolder}/{fileName}";
    }

    public Task DeleteAsync(string? publicPath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(publicPath))
        {
            return Task.CompletedTask;
        }

        var basePath = _settings.PublicBasePath.TrimEnd('/');
        if (!publicPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
        {
            return Task.CompletedTask;
        }

        var relativePart = publicPath[basePath.Length..].TrimStart('/');
        var physicalPath = Path.Combine(_settings.RootPath, relativePart.Replace('/', Path.DirectorySeparatorChar));

        try
        {
            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }
        }
        catch (IOException)
        {
        }
        return Task.CompletedTask;
    }
}
