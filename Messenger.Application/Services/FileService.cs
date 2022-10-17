using Messenger.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Messenger.Application.Services;

public class FileService : IFileService
{
	public async Task<string> CreateFileAsync(string path, IFormFile file, string domainName)
	{
		var fileName = $"{Guid.NewGuid().ToString()}.jpeg";
		Directory.CreateDirectory(path);

		await using (var stream = new FileStream(Path.Combine(path, fileName), FileMode.OpenOrCreate))
		{
			await file.CopyToAsync(stream);
		}

		return $"{domainName}/{fileName}";
	}

	public void DeleteFile(string path)
	{
		File.Delete(path);
	}

	public bool IsFileExtension(IFormFile file, params string[] extensions)
	{
		return extensions.Contains(file.ContentType);
	}
}