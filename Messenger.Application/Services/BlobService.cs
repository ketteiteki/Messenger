using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Messenger.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Messenger.Application.Services;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IBlobServiceSettings _blobServiceSettings;

    public BlobService(BlobServiceClient blobServiceClient, IBlobServiceSettings blobServiceSettings)
    {
        _blobServiceClient = blobServiceClient;
        _blobServiceSettings = blobServiceSettings;
    }

    public Task<string> GetBlobAsync(string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobServiceSettings.MessengerBlobContainerName);
        var client = containerClient.GetBlobClient(fileName);

        return Task.FromResult(client.Uri.AbsoluteUri);
    }

    public async Task<string> UploadFileBlobAsync(IFormFile file)
    {
        using (var stream = file.OpenReadStream())
        {
            var uniqueName = Guid.NewGuid() + $".{file.ContentType.Split("/")[^1]}";
        
            var blobContainerName = _blobServiceSettings.MessengerBlobContainerName;
            var containerClient = GetContainerClient(blobContainerName);
            var client = containerClient.GetBlobClient(uniqueName);
            var headers = new BlobHttpHeaders { ContentType = file.ContentType };
            await client.UploadAsync(stream, headers);

            return client.Uri.AbsoluteUri;
        }
    }

    public async Task<bool> DeleteBlobAsync(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobServiceSettings.MessengerBlobContainerName);
        var client = containerClient.GetBlobClient(fileName);

        var result = await client.DeleteIfExistsAsync();

        return result.Value;
    }

    private BlobContainerClient GetContainerClient(string blobContainerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
        containerClient.CreateIfNotExists();
        return containerClient;
    }
}