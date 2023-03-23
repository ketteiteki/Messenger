import { RoleColor } from '../enum/RoleColor';


export default interface IRoleUserByChatDto {
    userId: string;
    chatId: string;
    roleTitle: string | null;
    roleColor: RoleColor;
    canBanUser: boolean;
    canChangeChatData: boolean;
    canGivePermissionToUser: boolean;
    canAddAndRemoveUserToConversation: boolean;
    isOwner: boolean;
}