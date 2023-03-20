using Microsoft.AspNetCore.Http;

namespace Messenger.Application.Interfaces;

public interface IBlobService
{
    Task<string> GetBlobAsync(string fileName);

    Task<string> UploadFileBlobAsync(IFormFile file);

    Task<bool> DeleteBlobAsync(string fileName);
}