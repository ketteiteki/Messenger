namespace Messenger.Application.Interfaces;

public interface IBlobServiceSettings
{
    string MessengerBlobContainerName { get; }
    
    string MessengerBlobAccess { get; }
}