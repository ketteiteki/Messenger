namespace Messenger.Domain.Exceptions;

public class StoreException : Exception
{
    public StoreException(string message) : base(message) {}
}