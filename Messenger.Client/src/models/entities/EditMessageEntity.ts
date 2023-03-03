


export default class EditMessageEntity {
  public messageId: string;
  public chatId: string;
  public displayName: string;
  public text: string;

  constructor(messageId: string, chatId: string, displayName: string, text: string) {
    this.messageId = messageId;
    this.chatId = chatId;
    this.displayName = displayName;
    this.text = text;
  }
}