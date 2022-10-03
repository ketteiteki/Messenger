using Messenger.Application.Interfaces;
using Messenger.Domain.Constants;
using Microsoft.AspNetCore.Http;

namespace Messenger.Application.Services;

public class FileService : IFileService
{
	public async Task<string> CreateFileAsync(string path, IFormFile file)
	{
		var fileName = $"{Guid.NewGuid().ToString()}.jpeg";
		Directory.CreateDirectory(path);
		
		using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.OpenOrCreate))
		{
			await file.CopyToAsync(stream);
		}

		return $"{EnvironmentConstants.DomainName}/{fileName}";
	}

	public void DeleteFile(string path)
	{
		File.Delete(path);
	}

	public bool IsFileExtension(IFormFile file, params string[] extentions)
	{
		return extentions.Contains(file.ContentType);
	}
}