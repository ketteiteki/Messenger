using Microsoft.AspNetCore.Http;

namespace Messenger.BusinessLogic.Models.Requests;

public class UpdateChatAvatarRequest
{
    public Guid ChatId { get; set; }

    public IFormFile AvatarFile { get; set; }
}