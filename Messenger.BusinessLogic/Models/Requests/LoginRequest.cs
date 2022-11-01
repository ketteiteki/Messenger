namespace Messenger.BusinessLogic.Models.Requests;

public class LoginRequest
{
	public string Nickname { get; set; } = null!;
	
	public string Password { get; set; } = null!;
}