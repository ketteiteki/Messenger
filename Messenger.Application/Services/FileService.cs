using Messenger.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Messenger.Application.Services;

public class FileService : IFileService
{
	public async Task<string> CreateFileAsync(string pathWwwRoot, IFormFile file, string domainName)
	{
		var fileName = $"{Guid.NewGuid().ToString()}.jpeg";
		Directory.CreateDirectory(pathWwwRoot);

		await using (var stream = new FileStream(Path.Combine(pathWwwRoot, fileName), FileMode.OpenOrCreate))
		{
			await file.CopyToAsync(stream);
		}

		return $"{domainName}/{fileName}";
	}

	public void DeleteFile(string avatarFilePath)
	{
		File.Delete(avatarFilePath);
	}

	public bool IsFileExtension(IFormFile file, params string[] extensions)
	{
		return extensions.Contains(file.ContentType);
	}
}