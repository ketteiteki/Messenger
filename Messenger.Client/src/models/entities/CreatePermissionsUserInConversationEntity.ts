


export default class CreatePermissionsUserInConversationEntity {
  public chatId: string;
  public userId: string;
  public canSendMedia: boolean;
  public muteMinutes: number | null;

  constructor(chatId: string, userId: string, canSendMedia: boolean, muteMinutes: number | null) {
    this.chatId = chatId;
    this.userId = userId;
    this.canSendMedia = canSendMedia;
    this.muteMinutes = muteMinutes;
  }
}