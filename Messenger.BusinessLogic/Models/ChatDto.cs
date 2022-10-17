using Messenger.Domain.Entities;
using Messenger.Domain.Enum;

namespace Messenger.BusinessLogic.Models;

public class ChatDto
{
	public Guid Id { get; init; }

	public string Name { get; init; }

	public string Title { get; init; }

	public ChatType Type { get; init; }
	
	public string AvatarLink { get; init; }
	
	public int MembersCount { get; init; }
	
	public bool CanSendMedia { get; init; }
	
	public bool IsOwner { get; init; }
	
	public bool IsMember { get; init; }
	
	public DateTime? MuteDateOfExpire { get; init; }

	public DateTime? BanDateOfExpire { get; init; }
	
	public RoleUserByChatDto RoleUser { get; init; }
	
	public List<UserDto> Members { get; set; }
}