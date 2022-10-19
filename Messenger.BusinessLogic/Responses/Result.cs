using Messenger.BusinessLogic.Responses.Abstractions;

namespace Messenger.BusinessLogic.Responses;

public class Result<TValue>
{
	public bool IsSuccess { get; set; } = true;

	public TValue Value { get; set; }
	
	public Error Error { get; set; }

	public Result(TValue value)
	{
		Value = value;
	}

	public Result(Error error)
	{
		Error = error;
		IsSuccess = false;
	}
}