using Messenger.Domain.Entities;

namespace Messenger.BusinessLogic.Models.RequestResponse;

public class RegistrationResponse
{
	public string AccessToken { get; set; } = null!;
	
	public Guid Id { get; set; }
	
	public string DisplayName { get; set; } = null!;
	
	public string Nickname { get; set; } = null!;

	public string AvatarLink { get; set; }

	public RegistrationResponse(User user, string accessToken)
	{
		AccessToken = accessToken;
		Id = user.Id;
		DisplayName = user.DisplayName;
		Nickname = user.NickName;
	}
}