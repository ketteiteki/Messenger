import IAttachmentDto from "../interfaces/IAttachmentDto";
import IMessageDto from "../interfaces/IMessageDto";

export default class MessageEntity implements IMessageDto {
  public id: string = `${Math.random()}`;
  public text: string;
  public isEdit: boolean = false;
  public ownerId: string | null;
  public ownerDisplayName: string | null;
  public ownerAvatarLink: string | null;
  public replyToMessageId: string | null;
  public replyToMessageText: string | null;
  public replyToMessageAuthorDisplayName: string | null;
  public attachments: IAttachmentDto[] = [];
  public chatId: string;
  public dateOfCreate: string = `${new Date()}`;

  public loading: boolean = false;
  public isMessageRealtime: boolean = false;

  constructor(
    text: string,
    ownerId: string,
    ownerDisplayName: string,
    ownerAvatarLink: string | null,
    replyToMessageId: string | null,
    replyToMessageText: string | null,
    replyToMessageAuthorDisplayName: string | null,
    chatId: string,
    attachments: IAttachmentDto[],
    loading: boolean = false,
    isMessageRealtime: boolean = false
  ) {
    this.text = text;
    this.ownerId = ownerId;
    this.ownerDisplayName = ownerDisplayName;
    this.ownerAvatarLink = ownerAvatarLink;
    this.replyToMessageId = replyToMessageId;
    this.replyToMessageText = replyToMessageText;
    this.replyToMessageAuthorDisplayName = replyToMessageAuthorDisplayName;
    this.chatId = chatId;
    this.attachments = attachments;
    this.loading = loading;
    this.isMessageRealtime = isMessageRealtime;
  }
}
