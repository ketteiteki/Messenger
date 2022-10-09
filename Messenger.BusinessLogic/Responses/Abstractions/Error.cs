namespace Messenger.BusinessLogic.Responses.Abstractions;

public class Error
{
	public string Message { get; set; }

	public Error(string message)
	{
		Message = message;
	}
}