namespace Market.API.Services.Interfaces;

public interface IUploadFileService
{
    /// <summary>
    /// Uploads a file to the specified container in Blob Storage and returns the file URI.
    /// </summary>
    /// <param name="fileStream">The file stream to upload.</param>
    /// <param name="fileName">The name of the file to upload.</param>
    /// <param name="contentType">The content type of the file.</param>
    /// <param name="bucketName">The name of the container to upload the file to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The URI of the uploaded file.</returns>
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder, string contentType, string bucketName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from the specified container in Blob Storage.
    /// </summary>
    /// <param name="fileUrl">The URL of the file to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    Task<bool> DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);
}