namespace Messenger.Domain.Constants;

public static class AppSettingConstants
{
    public const string AllowedHosts = "AllowedHosts";
    
    public const string MessengerDomainName = "MESSENGER_DOMAIN_NAME";

    public const string MessengerJwtSettingsSecretAccessTokenKey = "MESSENGER_JWT_SETTINGS_SECRET_ACCESS_TOKEN_KEY"; 
    
    public const string DatabaseConnectionString = "MESSENGER_DATABASE_CONNECTION_STRING"; 

    public const string DatabaseConnectionStringForIntegrationTests = "MESSENGER_DATABASE_CONNECTION_STRING_INTEGRATION_TESTS"; 
}