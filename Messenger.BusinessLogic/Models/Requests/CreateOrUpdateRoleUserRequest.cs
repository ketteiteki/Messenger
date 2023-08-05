using Messenger.Domain.Enums;

namespace Messenger.BusinessLogic.Models.Requests;

public class CreateOrUpdateRoleUserRequest
{
    public Guid ChatId { get; set; }
    
    public Guid UserId { get; set; }
    
    public string RoleTitle { get; set; }
        
    public RoleColor RoleColor { get; set; }
        
    public bool CanBanUser { get; set; }

    public bool CanChangeChatData { get; set; }

    public bool CanAddAndRemoveUserToConversation { get; set; }

    public bool CanGivePermissionToUser { get; set; }
}