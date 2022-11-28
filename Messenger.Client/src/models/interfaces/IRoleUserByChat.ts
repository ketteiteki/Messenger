export interface IRoleUserByChat {
  userId: string;
  chatId: string;
  roleTitle: string;
  roleColor: number;
  canBanUser: boolean;
  canChangeChatData: boolean;
  canGivePermissionToUser: boolean;
  canAddAndRemoveUserToConversation: boolean;
  isOwner: boolean;
}
