
namespace Messenger.BusinessLogic.Models.Responses;

public class AuthorizationResponse
{
	public string AccessToken { get; set; } 

	public Guid RefreshToken { get; set; } 
	
	public Guid Id { get; set; }
	
	public string DisplayName { get; set; }
	
	public string Nickname { get; set; } 
	
	public string Bio { get; set; }

	public string AvatarLink { get; set; }
	
	public AuthorizationResponse(
		string accessToken,
		Guid refreshToken,
		Guid id,
		string displayName,
		string nickname, 
		string bio,
		string avatarLink)
	{
		AccessToken = accessToken;
		RefreshToken = refreshToken;
		Id = id;
		DisplayName = displayName;
		Nickname = nickname;
		Bio = bio;
		AvatarLink = avatarLink;
	}
}