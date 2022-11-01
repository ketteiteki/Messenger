namespace Messenger.BusinessLogic.Models.Requests;

public class RegistrationRequest
{
	public string DisplayName { get; set; } = null!;
	
	public string Nickname { get; set; } = null!;

	public string Password { get; set; } = null!;
}