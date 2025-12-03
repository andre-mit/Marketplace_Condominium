using Amazon.S3;
using Amazon.S3.Model;
using Market.API.SettingsModels;
using Microsoft.Extensions.Options;

namespace Market.API.Services;

public class UploadFileService(ILogger<UploadFileService> logger, IAmazonS3 s3Client, IOptions<S3Config> options)
    : IUploadFileService
{
    private readonly S3Config _settings = options.Value;

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder, string contentType, string bucketName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if(!folder.IsNullOrWhiteSpace())
            {
                fileName = $"{folder.TrimEnd('/')}/{fileName}";
            }
            
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = fileName,
                InputStream = fileStream,
                ContentType = contentType
            };

            await s3Client.PutObjectAsync(request, cancellationToken);

            var url = $"{_settings.ServiceUrl}/{bucketName}/{fileName}";

            logger.LogInformation("File uploaded to {Url}", url);
            return url;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error uploading file to Blob Storage");
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string fileUrl,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var uri = new Uri(fileUrl);
            var segments = uri.Segments;
            if (segments.Length < 3)
            {
                throw new ArgumentException("Invalid file URL", nameof(fileUrl));
            }
            
            var bucketName = segments[1].TrimEnd('/');
            var key = string.Join("", segments.Skip(2));
            var request = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };
            
            await s3Client.DeleteObjectAsync(request, cancellationToken);
            
            logger.LogInformation("File deleted from {Url}", fileUrl);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting file from Blob Storage");
            return false;
        }
    }
}