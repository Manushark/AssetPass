namespace AssetPass.Application.Interfaces;

/// <summary>
/// Abstraction for file storage operations.
/// Local implementation for dev, cloud-ready for production.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Stores a file and returns the storage path.
    /// </summary>
    Task<string> StoreFileAsync(Stream fileStream, string fileName, string contentType, string folder, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a file by its storage path.
    /// </summary>
    Task<Stream?> GetFileAsync(string storagePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file by its storage path.
    /// </summary>
    Task<bool> DeleteFileAsync(string storagePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a file exists.
    /// </summary>
    Task<bool> FileExistsAsync(string storagePath, CancellationToken cancellationToken = default);
}
