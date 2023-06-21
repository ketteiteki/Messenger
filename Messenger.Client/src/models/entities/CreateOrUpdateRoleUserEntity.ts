import { RoleColor } from "../enum/RoleColor";


export default class CreateOrUpdateRoleUserEntity {
  public chatId: string;
  public userId: string;
  public roleTitle: string;
  public roleColor: RoleColor;
  public canBanUser: boolean;
  public canChangeChatData: boolean;
  public canAddAndRemoveUserToConversation: boolean;
  public canGivePermissionToUser: boolean;

  constructor(
    chatId: string, 
    userId: string, 
    roleTitle: string, 
    roleColor: RoleColor, 
    canBanUser: boolean, 
    canChangeChatData: boolean, 
    canAddAndRemoveUserToConversation: boolean, 
    canGivePermissionToUser: boolean) {
      this.chatId = chatId;
      this.userId = userId;
      this.roleTitle = roleTitle;
      this.roleColor = roleColor;
      this.canBanUser = canBanUser;
      this.canChangeChatData = canChangeChatData;
      this.canAddAndRemoveUserToConversation = canAddAndRemoveUserToConversation;
      this.canGivePermissionToUser = canGivePermissionToUser;
  }
}