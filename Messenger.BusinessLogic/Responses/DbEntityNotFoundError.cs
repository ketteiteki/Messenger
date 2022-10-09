using Messenger.BusinessLogic.Responses.Abstractions;

namespace Messenger.BusinessLogic.Responses;

public class DbEntityNotFoundError : Error
{
	public DbEntityNotFoundError(string message) : base(message) {}
}