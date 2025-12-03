namespace Catalog_Service.src._02_Infrastructure.FileStorage
{
    public class LocalFileStorage : IFileStorage
    {
        private readonly string _storagePath;
        private readonly string _baseUrl;
        private readonly ILogger<LocalFileStorage> _logger;

        public LocalFileStorage(IConfiguration configuration, ILogger<LocalFileStorage> logger)
        {
            _storagePath = configuration["FileStorage:LocalPath"] ?? "uploads";
            _baseUrl = configuration["FileStorage:BaseUrl"] ?? "/uploads";
            _logger = logger;

            // Ensure directory exists
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
        {
            try
            {
                // Create unique filename to avoid conflicts
                var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                var filePath = Path.Combine(_storagePath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileStream.CopyToAsync(stream, cancellationToken);
                }

                _logger.LogInformation("File uploaded successfully: {FileName}", uniqueFileName);
                return $"{_baseUrl}/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file: {FileName}", fileName);
                throw;
            }
        }

        public async Task<Stream> DownloadFileAsync(string fileName, CancellationToken cancellationToken = default)
        {
            try
            {
                // Extract just the filename from the URL
                var actualFileName = Path.GetFileName(fileName);
                var filePath = Path.Combine(_storagePath, actualFileName);

                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("File not found: {FileName}", actualFileName);
                    throw new FileNotFoundException("File not found", actualFileName);
                }

                var memoryStream = new MemoryStream();
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    await fileStream.CopyToAsync(memoryStream, cancellationToken);
                }
                memoryStream.Position = 0;

                _logger.LogInformation("File downloaded successfully: {FileName}", actualFileName);
                return memoryStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file: {FileName}", fileName);
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string fileName, CancellationToken cancellationToken = default)
        {
            try
            {
                // Extract just the filename from the URL
                var actualFileName = Path.GetFileName(fileName);
                var filePath = Path.Combine(_storagePath, actualFileName);

                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("File not found for deletion: {FileName}", actualFileName);
                    return false;
                }

                File.Delete(filePath);
                _logger.LogInformation("File deleted successfully: {FileName}", actualFileName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FileName}", fileName);
                return false;
            }
        }

        public Task<string> GetFileUrlAsync(string fileName, CancellationToken cancellationToken = default)
        {
            // For local storage, the URL is just the base URL + filename
            return Task.FromResult($"{_baseUrl}/{Path.GetFileName(fileName)}");
        }

        public Task<bool> FileExistsAsync(string fileName, CancellationToken cancellationToken = default)
        {
            var actualFileName = Path.GetFileName(fileName);
            var filePath = Path.Combine(_storagePath, actualFileName);
            return Task.FromResult(File.Exists(filePath));
        }
    }
}
