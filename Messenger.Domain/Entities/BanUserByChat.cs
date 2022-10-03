namespace Messenger.Domain.Entities;

public class BanUserByChat
{
	public Guid UserId { get; set; }

	public User User { get; set; }
	
	public Guid ChatId { get; set; }
	
	public Chat Chat { get; set; }
	
	public DateTime BanDateOfExpire { get; set; } 
}
