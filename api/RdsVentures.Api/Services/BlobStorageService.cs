using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace RdsVentures.Api.Services;

public interface IBlobStorageService
{
    Task<(string sasUrl, string blobUrl)> GenerateSasTokenAsync(string fileName, string contentType);
}

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public BlobStorageService(BlobServiceClient blobServiceClient, IConfiguration configuration)
    {
        _blobServiceClient = blobServiceClient;
        _containerName = configuration["Azure:BlobStorage:ContainerName"] ?? "attachments";
    }

    public async Task<(string sasUrl, string blobUrl)> GenerateSasTokenAsync(string fileName, string contentType)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        
        // Ensure container exists
        await containerClient.CreateIfNotExistsAsync();

        var blobName = $"{Guid.NewGuid()}-{fileName}";
        var blobClient = containerClient.GetBlobClient(blobName);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            BlobName = blobName,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Write | BlobSasPermissions.Create);

        var sasToken = blobClient.GenerateSasUri(sasBuilder).ToString();
        var blobUrl = blobClient.Uri.ToString();

        return (sasToken, blobUrl);
    }
}
