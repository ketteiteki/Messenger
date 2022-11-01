using Microsoft.AspNetCore.Http;

namespace Messenger.BusinessLogic.Models.Requests;

public class UpdateProfileAvatar
{
    public IFormFile Avatar { get; set; }
}