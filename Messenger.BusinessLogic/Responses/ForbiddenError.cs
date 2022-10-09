using Messenger.BusinessLogic.Responses.Abstractions;

namespace Messenger.BusinessLogic.Responses;

public class ForbiddenError : Error
{
	public ForbiddenError(string message) : base(message) {}
}