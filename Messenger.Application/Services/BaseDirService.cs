using Messenger.Application.Interfaces;

namespace Messenger.Application.Services;

public class BaseDirService : IBaseDirService
{
	public string GetPathWwwRoot() => Path.Combine(AppContext.BaseDirectory, @"../../../../Messenger.WebApi/wwwroot");

	public string GetPathAppSettingsJson(bool isDevelopment)
	{
		if (isDevelopment)
			return Path.Combine(AppContext.BaseDirectory, "../../../../Messenger.WebApi/appsettings.Development.json");

		return Path.Combine(AppContext.BaseDirectory, "../../../../Messenger.WebApi/appsettings.json");
	}
}