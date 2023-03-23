



export default class ReplyEntity {
  public messageId: string;
  public displayName: string;
  public text: string;

  constructor(messageId: string, displayName: string, text: string) {
    this.messageId = messageId;
    this.displayName = displayName;
    this.text = text;
  }
}