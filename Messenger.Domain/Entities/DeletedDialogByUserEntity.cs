namespace Messenger.Domain.Entities;

public class DeletedDialogByUserEntity
{
	public Guid ChatId { get; set; }
	
	public ChatEntity Chat { get; set; }

	public Guid UserId { get; set; }
	
	public UserEntity User { get; set; }

	public DeletedDialogByUserEntity(Guid userId, Guid chatId)
	{
		UserId = userId;
		ChatId = chatId;
	}
}