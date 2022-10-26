using Microsoft.AspNetCore.Http;

namespace Messenger.BusinessLogic.Models.Requests;

public class UpdateConversationAvatarRequest
{
    public Guid ChatId { get; set; }

    public IFormFile Avatar { get; set; }
}