using Messenger.Application.Interfaces;

namespace Messenger.Application.Services;

public class BaseDirService : IBaseDirService
{
	public string GetPathWwwRoot() => Path.Combine(AppContext.BaseDirectory, @"../../../../Messenger.WebApi/wwwroot");

	public string GetPathAppSettingsJson(bool isDocker)
	{
		if (isDocker)
			return Path.Combine(AppContext.BaseDirectory, "../../../../Messenger.WebApi/appsettings.Docker.json");

		return Path.Combine(AppContext.BaseDirectory, "../../../../Messenger.WebApi/appsettings.json");
	}
}