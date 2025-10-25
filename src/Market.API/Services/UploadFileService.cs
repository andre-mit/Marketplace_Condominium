using Azure.Storage.Blobs;

namespace Market.API.Services;

public class UploadFileService(ILogger<UploadFileService> logger, BlobServiceClient blobServiceClient)
    : IUploadFileService
{
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string containerName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid().ToString());
            await blobClient.UploadAsync(fileStream, cancellationToken);
            logger.LogInformation("File uploaded to {BlobUri}", blobClient.Uri);
            return blobClient.Uri.ToString();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error uploading file to Blob Storage");
            throw;
        }
    }

    public async Task DeleteFileAsync(string fileUri, string containerName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobName = new Uri(fileUri).Segments.Last();
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
            logger.LogInformation("File {BlobName} deleted from container {ContainerName}", blobName, containerName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting file from Blob Storage");
            throw;
        }
    }
}