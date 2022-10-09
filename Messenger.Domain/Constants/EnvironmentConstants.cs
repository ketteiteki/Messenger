namespace Messenger.Domain.Constants;

public static class EnvironmentConstants
{
	public const string DatabaseConnectionString = "Server=localhost:5432;Database=postgres;User Id=postgres;Password=root";
	
	public const string DomainName = "localhost:7400";
	
	public const string JwtIssuerSigningAccessKey = "secretAccessTokenKey_1231";
	
	public static string GetPathWWWRoot() => Path.Combine(AppContext.BaseDirectory, @"../../../../Messenger.WebApi/wwwroot");
}