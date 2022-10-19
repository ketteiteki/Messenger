using Messenger.Domain.Entities;

namespace Messenger.BusinessLogic.Models.RequestResponse;

public class RegistrationResponse
{
	public string AccessToken { get; set; }
	
	public Guid Id { get; set; }
	
	public string DisplayName { get; set; }
	
	public string Nickname { get; set; }

	public string Bio { get; set; }
	
	public string AvatarLink { get; set; }

	public RegistrationResponse(User user, string accessToken)
	{
		AccessToken = accessToken;
		Id = user.Id;
		DisplayName = user.DisplayName;
		Nickname = user.NickName;
		Bio = user.Bio;
	}
}