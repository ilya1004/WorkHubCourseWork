using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using IdentityService.BLL.Abstractions.BlobService;
using IdentityService.BLL.DTOs;
using IdentityService.BLL.Settings;

namespace IdentityService.BLL.Services.BlobService;

public class BlobService : IBlobService
{
    private readonly string _containerName;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<BlobService> _logger;

    public BlobService(
        BlobServiceClient blobServiceClient,
        IOptions<AzuriteSettings> options,
        ILogger<BlobService> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
        _containerName = options.Value.ImagesContainerName;
    }

    public async Task<Guid> UploadAsync(Stream stream, string contentType, CancellationToken cancellationToken = default)
    {
        try
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var fileId = Guid.CreateVersion7();
            var blobClient = blobContainerClient.GetBlobClient(fileId.ToString());

            await blobClient.UploadAsync(
                stream,
                new BlobHttpHeaders { ContentType = contentType },
                cancellationToken: cancellationToken);

            _logger.LogInformation("File {FileId} uploaded successfully. Size: {Size} bytes", fileId, stream.Length);

            return fileId;
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.ContainerNotFound)
        {
            _logger.LogError("Container {Container} not found", _containerName);
            
            throw new NotFoundException("The specified blob container does not exist.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "File upload failed");
            
            throw new BadRequestException($"An error occurred during the upload process: {ex.Message}");
        }
    }

    public async Task<FileResponseDto> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        try
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = blobContainerClient.GetBlobClient(fileId.ToString());

            var fileResponse = await blobClient.DownloadContentAsync(cancellationToken);

            _logger.LogInformation("File {FileId} downloaded successfully. Content type: {ContentType}",
                fileId, fileResponse.Value.Details.ContentType);

            return new FileResponseDto(
                fileResponse.Value.Content.ToStream(),
                fileResponse.Value.Details.ContentType);
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            _logger.LogError("File {FileId} not found in container {Container}", fileId, _containerName);
            throw new NotFoundException($"The file with ID {fileId} not found in the blob storage.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "File download failed for {FileId}", fileId);
            throw new BadRequestException($"An error occurred during the download process: {ex.Message}");
        }
    }

    public async Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        try
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = blobContainerClient.GetBlobClient(fileId.ToString());

            var result = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

            if (result.Value)
            {
                _logger.LogInformation("File {FileId} deleted successfully", fileId);
            }
            else
            {
                _logger.LogError("File {FileId} did not exist when attempting to delete", fileId);
            }
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            _logger.LogError("File {FileId} not found during deletion attempt", fileId);
            
            throw new NotFoundException($"The file with ID {fileId} not found in the blob storage.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "File deletion failed for {FileId}", fileId);
            
            throw new BadRequestException($"An error occurred during the deletion process: {ex.Message}");
        }
    }
}