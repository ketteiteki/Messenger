namespace Messenger.BusinessLogic.Exceptions;

public class DbEntityExistsException : Exception
{
	public DbEntityExistsException(string message) : base(message) {}
}