using Microsoft.AspNetCore.Http;

namespace Messenger.Application.Interfaces;

public interface IFileService
{
	public Task<string> CreateFileAsync(string path, IFormFile file);

	public void DeleteFile(string path);

	public bool IsFileExtension(IFormFile file, params string[] extentions);
}