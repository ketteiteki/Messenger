namespace Messenger.BusinessLogic.Models;

public class NotifyBanUserDto
{
    public Guid ChatId { get; set; }

    public DateTime BanDateOfExpire { get; set; }

    public NotifyBanUserDto(Guid chatId, DateTime banDateOfExpire)
    {
        ChatId = chatId;
        BanDateOfExpire = banDateOfExpire;
    }
}