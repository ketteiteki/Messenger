using Microsoft.AspNetCore.Http;

namespace Messenger.BusinessLogic.Models.Requests;

public class UpdateProfileAvatar
{
    public Guid Id { get; set; }
    
    public IFormFile Avatar { get; set; }
}