namespace Messenger.BusinessLogic.Exceptions;

public class DbEntityNotFoundException : Exception
{
	public DbEntityNotFoundException(string message) : base(message) {}
}