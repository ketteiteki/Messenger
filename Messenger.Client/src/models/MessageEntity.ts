import { IAttachment } from "./interfaces/IAttachment";
import { IMessage } from "./interfaces/IMessage";

export default class MessageEntity implements IMessage {
  public id;
  public text;
  public isEdit;
  public ownerId;
  public ownerDisplayName;
  public ownerAvatarLink;
  public replyToMessageId;
  public replyToMessageText;
  public replyToMessageAuthorDisplayName;
  public attachments;
  public chatId;
  public dateOfCreate;

  constructor(
    id: string,
    text: string,
    isEdit: boolean,
    ownerId: string,
    ownerDisplayName: string,
    ownerAvatarLink: string | null,
    replyToMessageId: string | null,
    replyToMessageText: string | null,
    replyToMessageAuthorDisplayName: string | null,
    attachments: IAttachment[],
    chatId: string,
    dateOfCreate: string
  ) {
    this.id = id;
    this.text = text;
    this.isEdit = isEdit;
    this.ownerId = ownerId;
    this.ownerDisplayName = ownerDisplayName;
    this.ownerAvatarLink = ownerAvatarLink;
    this.replyToMessageId = replyToMessageId;
    this.replyToMessageText = replyToMessageText;
    this.replyToMessageAuthorDisplayName = replyToMessageAuthorDisplayName;
    this.attachments = attachments;
    this.chatId = chatId;
    this.dateOfCreate = dateOfCreate;
  }
}
