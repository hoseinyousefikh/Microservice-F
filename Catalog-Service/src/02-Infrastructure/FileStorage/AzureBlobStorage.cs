using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Catalog_Service.src._02_Infrastructure.FileStorage
{
    public class AzureBlobStorage : IFileStorage
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        private readonly ILogger<AzureBlobStorage> _logger;

        public AzureBlobStorage(IConfiguration configuration, ILogger<AzureBlobStorage> logger)
        {
            var connectionString = configuration["FileStorage:Azure:ConnectionString"];
            _containerName = configuration["FileStorage:Azure:ContainerName"];
            _logger = logger;

            _blobServiceClient = new BlobServiceClient(connectionString);

            // Ensure container exists
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            containerClient.CreateIfNotExistsAsync().GetAwaiter().GetResult();
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                var blobClient = containerClient.GetBlobClient(uniqueFileName);

                await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);

                _logger.LogInformation("File uploaded to Azure Blob Storage successfully: {FileName}", uniqueFileName);
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file to Azure Blob Storage: {FileName}", fileName);
                throw;
            }
        }

        public async Task<Stream> DownloadFileAsync(string fileName, CancellationToken cancellationToken = default)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                var actualFileName = Path.GetFileName(fileName);
                var blobClient = containerClient.GetBlobClient(actualFileName);

                if (!await blobClient.ExistsAsync(cancellationToken))
                {
                    _logger.LogWarning("File not found in Azure Blob Storage: {FileName}", actualFileName);
                    throw new FileNotFoundException("File not found", actualFileName);
                }

                var blobDownloadInfo = await blobClient.DownloadAsync(cancellationToken);
                var memoryStream = new MemoryStream();
                await blobDownloadInfo.Value.Content.CopyToAsync(memoryStream, cancellationToken);
                memoryStream.Position = 0;

                _logger.LogInformation("File downloaded from Azure Blob Storage successfully: {FileName}", actualFileName);
                return memoryStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file from Azure Blob Storage: {FileName}", fileName);
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string fileName, CancellationToken cancellationToken = default)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                var actualFileName = Path.GetFileName(fileName);
                var blobClient = containerClient.GetBlobClient(actualFileName);

                if (!await blobClient.ExistsAsync(cancellationToken))
                {
                    _logger.LogWarning("File not found for deletion in Azure Blob Storage: {FileName}", actualFileName);
                    return false;
                }

                await blobClient.DeleteAsync(cancellationToken: cancellationToken);
                _logger.LogInformation("File deleted from Azure Blob Storage successfully: {FileName}", actualFileName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file from Azure Blob Storage: {FileName}", fileName);
                return false;
            }
        }

        public Task<string> GetFileUrlAsync(string fileName, CancellationToken cancellationToken = default)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var actualFileName = Path.GetFileName(fileName);
            var blobClient = containerClient.GetBlobClient(actualFileName);
            return Task.FromResult(blobClient.Uri.ToString());
        }

        public async Task<bool> FileExistsAsync(string fileName, CancellationToken cancellationToken = default)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                var actualFileName = Path.GetFileName(fileName);
                var blobClient = containerClient.GetBlobClient(actualFileName);
                return await blobClient.ExistsAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking file existence in Azure Blob Storage: {FileName}", fileName);
                return false;
            }
        }
    }
}
