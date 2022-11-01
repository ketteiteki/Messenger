using Messenger.Domain.Entities;

namespace Messenger.BusinessLogic.Models;

public class UserDto
{
	public Guid Id { get; init; }
	
	public string DisplayName { get; init; }

	public string Nickname { get; init; }

	public string Bio { get; init; }

	public string AvatarLink { get; init; }

	public UserDto(User user)
	{
		Id = user.Id;
		DisplayName = user.DisplayName;
		Nickname = user.Nickname;
		Bio = user.Bio;
		AvatarLink = user.AvatarLink;
	}
}