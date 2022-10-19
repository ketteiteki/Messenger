using Messenger.Domain.Entities;

namespace Messenger.BusinessLogic.Models.RequestResponse;

public class AuthorizationResponse
{
	public string AccessToken { get; set; } 
	
	public Guid Id { get; set; }
	
	public string DisplayName { get; set; }
	
	public string NickName { get; set; } 
	
	public string Bio { get; set; }

	public string AvatarLink { get; set; }
	
	public AuthorizationResponse(User user, string accessToken)
	{
		AccessToken = accessToken;
		Id = user.Id;
		DisplayName = user.DisplayName;
		NickName = user.NickName;
		Bio = user.Bio;
		AvatarLink = user.AvatarLink;
	}
}