namespace Messenger.Domain.Constants;

public static class BaseDirService
{
	public static string GetPathWwwRoot() => Path.Combine(AppContext.BaseDirectory, @"../../../../Messenger.WebApi/wwwroot");
}