using Messenger.Application.Interfaces;

namespace Messenger.Application.Services;

public class BlobServiceSettings : IBlobServiceSettings
{
    public string MessengerBlobContainerName { get; }
    
    public string MessengerBlobAccess { get; }

    public BlobServiceSettings(string messengerBlobContainerName, string messengerBlobAccess)
    {
        MessengerBlobContainerName = messengerBlobContainerName;
        MessengerBlobAccess = messengerBlobAccess;
    }
}