namespace Catalog_Service.src._02_Infrastructure.FileStorage
{
    public interface IFileStorage
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
        Task<Stream> DownloadFileAsync(string fileName, CancellationToken cancellationToken = default);
        Task<bool> DeleteFileAsync(string fileName, CancellationToken cancellationToken = default);
        Task<string> GetFileUrlAsync(string fileName, CancellationToken cancellationToken = default);
        Task<bool> FileExistsAsync(string fileName, CancellationToken cancellationToken = default);
    }
}
