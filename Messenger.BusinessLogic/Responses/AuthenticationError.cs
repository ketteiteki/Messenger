using Messenger.BusinessLogic.Responses.Abstractions;

namespace Messenger.BusinessLogic.Responses;

public class AuthenticationError : Error
{
	public AuthenticationError(string message) : base(message) { }
}