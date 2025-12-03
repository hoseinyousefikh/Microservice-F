using Amazon.S3;
using Amazon.S3.Model;

namespace Catalog_Service.src._02_Infrastructure.FileStorage
{
    public class S3FileStorage : IFileStorage
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly string _region;
        private readonly ILogger<S3FileStorage> _logger;

        public S3FileStorage(IConfiguration configuration, ILogger<S3FileStorage> logger)
        {
            _bucketName = configuration["FileStorage:S3:BucketName"];
            _region = configuration["FileStorage:S3:Region"];
            _logger = logger;

            var s3Config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(_region)
            };

            _s3Client = new AmazonS3Client(s3Config);
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
        {
            try
            {
                var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = uniqueFileName,
                    InputStream = fileStream,
                    ContentType = contentType,
                    CannedACL = S3CannedACL.PublicRead
                };

                await _s3Client.PutObjectAsync(putRequest, cancellationToken);

                var fileUrl = $"https://{_bucketName}.s3.{_region}.amazonaws.com/{uniqueFileName}";
                _logger.LogInformation("File uploaded to S3 successfully: {FileName}", uniqueFileName);
                return fileUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file to S3: {FileName}", fileName);
                throw;
            }
        }

        public async Task<Stream> DownloadFileAsync(string fileName, CancellationToken cancellationToken = default)
        {
            try
            {
                var actualFileName = Path.GetFileName(fileName);
                var getRequest = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = actualFileName
                };

                using (var response = await _s3Client.GetObjectAsync(getRequest, cancellationToken))
                {
                    var memoryStream = new MemoryStream();
                    await response.ResponseStream.CopyToAsync(memoryStream, cancellationToken);
                    memoryStream.Position = 0;

                    _logger.LogInformation("File downloaded from S3 successfully: {FileName}", actualFileName);
                    return memoryStream;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file from S3: {FileName}", fileName);
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string fileName, CancellationToken cancellationToken = default)
        {
            try
            {
                var actualFileName = Path.GetFileName(fileName);
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = actualFileName
                };

                var response = await _s3Client.DeleteObjectAsync(deleteRequest, cancellationToken);

                _logger.LogInformation("File deleted from S3 successfully: {FileName}", actualFileName);
                return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file from S3: {FileName}", fileName);
                return false;
            }
        }

        public Task<string> GetFileUrlAsync(string fileName, CancellationToken cancellationToken = default)
        {
            var actualFileName = Path.GetFileName(fileName);
            var fileUrl = $"https://{_bucketName}.s3.{_region}.amazonaws.com/{actualFileName}";
            return Task.FromResult(fileUrl);
        }

        public async Task<bool> FileExistsAsync(string fileName, CancellationToken cancellationToken = default)
        {
            try
            {
                var actualFileName = Path.GetFileName(fileName);
                var request = new GetObjectMetadataRequest
                {
                    BucketName = _bucketName,
                    Key = actualFileName
                };

                await _s3Client.GetObjectMetadataAsync(request, cancellationToken);
                return true;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking file existence in S3: {FileName}", fileName);
                return false;
            }
        }
    }
}
