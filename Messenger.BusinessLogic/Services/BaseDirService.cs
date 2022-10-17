namespace Messenger.BusinessLogic.Services;

public static class BaseDirService
{
	public static string GetPathWwwRoot() => Path.Combine(AppContext.BaseDirectory, @"../../../../Messenger.WebApi/wwwroot");

	public static string GetPathAppSettingsJson(bool isDevelopment)
	{
		if (isDevelopment)
			return Path.Combine(AppContext.BaseDirectory, "../../../../Messenger.WebApi/appsettings.Development.json");

		return Path.Combine(AppContext.BaseDirectory, "../../../../Messenger.WebApi/appsettings.json");
	}
}