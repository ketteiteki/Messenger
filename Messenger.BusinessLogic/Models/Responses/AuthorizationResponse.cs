using Messenger.Domain.Entities;

namespace Messenger.BusinessLogic.Models.Responses;

public class AuthorizationResponse
{
	public string AccessToken { get; set; } 

	public Guid RefreshToken { get; set; } 
	
	public Guid Id { get; set; }
	
	public string DisplayName { get; set; }
	
	public string NickName { get; set; } 
	
	public string Bio { get; set; }

	public string AvatarLink { get; set; }
	
	public AuthorizationResponse(UserEntity user, string accessToken, Guid refreshToken)
	{
		AccessToken = accessToken;
		RefreshToken = refreshToken;
		Id = user.Id;
		DisplayName = user.DisplayName;
		NickName = user.Nickname;
		Bio = user.Bio;
		AvatarLink = user.AvatarLink;
	}
}