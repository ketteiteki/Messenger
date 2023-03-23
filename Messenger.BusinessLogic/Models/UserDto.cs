
namespace Messenger.BusinessLogic.Models;

public class UserDto
{
	public Guid Id { get; init; }
	
	public string DisplayName { get; init; }

	public string Nickname { get; init; }

	public string Bio { get; init; }

	public string AvatarLink { get; init; }

	public UserDto(Guid id, string displayName, string nickname, string bio, string avatarLink)
	{
		Id = id;
		DisplayName = displayName;
		Nickname = nickname;
		Bio = bio;
		AvatarLink = avatarLink;
	}
}