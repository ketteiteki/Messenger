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

    public async Task<string> GetBlobAsync(string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobServiceSettings.MessengerBlobContainerName);
        var client = containerClient.GetBlobClient(fileName);

        var blobExists = await client.ExistsAsync();

        if (!blobExists.Value)
        {
            throw new FileNotFoundException("Blob file was not found at storage account.");
        }

        return client.Uri.AbsoluteUri;
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

            return client.Name;
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

    public bool UploadFolderToBlob(string folderPath)
    {
        var containerClient = GetContainerClient(_blobServiceSettings.MessengerBlobContainerName);
        var combinePath = Path.Combine(AppContext.BaseDirectory, folderPath);
        var files = Directory.GetFiles(combinePath);

        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            var client = containerClient.GetBlobClient(fileName);
            var fileExists = client.Exists();

            if (fileExists.Value)
            {
                continue;
            }

            using var stream = File.OpenRead(file);
            var headers = new BlobHttpHeaders { ContentType = "image/jpg" };
            client.Upload(stream, headers);
        }

        return true;
    }

    private BlobContainerClient GetContainerClient(string blobContainerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
        containerClient.CreateIfNotExists();
        containerClient.SetAccessPolicy(PublicAccessType.BlobContainer);

        return containerClient;
    }
}