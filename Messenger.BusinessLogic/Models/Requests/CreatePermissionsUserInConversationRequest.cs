namespace Messenger.BusinessLogic.Models.Requests;

public class CreatePermissionsUserInConversationRequest
{
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public bool CanSendMedia { get; set; }
    public int? MuteMinutes { get; set; }
}