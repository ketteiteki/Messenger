
namespace Messenger.BusinessLogic.Models.Responses;

public class AuthorizationResponse
{
	public Guid Id { get; private set; }
	
	public string DisplayName { get; private set; }
	
	public string Nickname { get; private set; } 
	
	public string Bio { get; private set; }

	public string AvatarLink { get; private set; }
	
	public Guid CurrentSessionId { get; set; }
	
	public AuthorizationResponse(
		Guid id,
		string displayName,
		string nickname, 
		string bio,
		string avatarLink)
	{
		Id = id;
		DisplayName = displayName;
		Nickname = nickname;
		Bio = bio;
		AvatarLink = avatarLink;
	}

	public void UpdateCurrentSessionId(Guid sessionId)
	{
		CurrentSessionId = sessionId;
	}
}