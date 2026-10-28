using AssetPass.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace AssetPass.Infrastructure.Services;

/// <summary>
/// Local file storage implementation for development.
/// Stores files in a configurable directory outside wwwroot.
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private readonly ILogger<LocalFileStorageService> _logger;

    // Allowed file types to prevent malicious uploads
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/gif", "image/webp",
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
    };

    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

    public LocalFileStorageService(string basePath, ILogger<LocalFileStorageService> logger)
    {
        _basePath = basePath;
        _logger = logger;
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> StoreFileAsync(Stream fileStream, string fileName, string contentType,
        string folder, CancellationToken cancellationToken = default)
    {
        // Validate content type
        if (!AllowedContentTypes.Contains(contentType))
        {
            throw new InvalidOperationException($"File type '{contentType}' is not allowed.");
        }

        // Validate file size
        if (fileStream.Length > MaxFileSizeBytes)
        {
            throw new InvalidOperationException($"File size exceeds maximum allowed size of {MaxFileSizeBytes / 1024 / 1024} MB.");
        }

        // Sanitize file name
        var safeFileName = SanitizeFileName(fileName);
        var uniqueName = $"{Guid.NewGuid():N}_{safeFileName}";
        var folderPath = Path.Combine(_basePath, folder);
        Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, uniqueName);
        var storagePath = Path.Combine(folder, uniqueName);

        await using var fileStreamDest = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(fileStreamDest, cancellationToken);

        _logger.LogInformation("File stored: {StoragePath}", storagePath);
        return storagePath;
    }

    public Task<Stream?> GetFileAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, storagePath);
        if (!File.Exists(fullPath))
        {
            return Task.FromResult<Stream?>(null);
        }

        // Prevent path traversal
        var resolvedPath = Path.GetFullPath(fullPath);
        if (!resolvedPath.StartsWith(Path.GetFullPath(_basePath)))
        {
            _logger.LogWarning("Path traversal attempt detected: {Path}", storagePath);
            return Task.FromResult<Stream?>(null);
        }

        Stream stream = new FileStream(resolvedPath, FileMode.Open, FileAccess.Read);
        return Task.FromResult<Stream?>(stream);
    }

    public Task<bool> DeleteFileAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, storagePath);
        var resolvedPath = Path.GetFullPath(fullPath);

        if (!resolvedPath.StartsWith(Path.GetFullPath(_basePath)))
        {
            return Task.FromResult(false);
        }

        if (File.Exists(resolvedPath))
        {
            File.Delete(resolvedPath);
            _logger.LogInformation("File deleted: {StoragePath}", storagePath);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public Task<bool> FileExistsAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, storagePath);
        return Task.FromResult(File.Exists(fullPath));
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        return sanitized.Length > 100 ? sanitized[..100] : sanitized;
    }
}
