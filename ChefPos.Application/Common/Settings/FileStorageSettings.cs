namespace ChefPos.Application.Common.Settings;

public class FileStorageSettings
{
    public const string SectionName = "FileStorage";

    public string RootPath { get; set; } = "uploads";
    public string PublicBasePath { get; set; } = "/uploads";
    public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024;
    public string[] AllowedContentTypes { get; set; } = { "image/jpeg", "image/png", "image/webp" };
    public string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".webp" };
}
