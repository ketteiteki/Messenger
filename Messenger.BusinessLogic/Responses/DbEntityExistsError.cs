using Messenger.BusinessLogic.Responses.Abstractions;

namespace Messenger.BusinessLogic.Responses;

public class DbEntityExistsError : Error
{
	public DbEntityExistsError(string message) : base(message) {}
}