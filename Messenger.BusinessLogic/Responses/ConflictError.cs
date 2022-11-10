using Messenger.BusinessLogic.Responses.Abstractions;

namespace Messenger.BusinessLogic.Responses;

public class ConflictError : Error
{
    public ConflictError(string message) : base(message)
    {
    }
}