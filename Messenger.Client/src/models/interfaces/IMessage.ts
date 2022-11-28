import { IAttachment } from "./IAttachment";

export interface IMessage {
  id: string;
  text: string;
  isEdit: boolean;
  ownerId: string | null;
  ownerDisplayName: string | null;
  ownerAvatarLink: string | null;
  replyToMessageId: string | null;
  replyToMessageText: string | null;
  replyToMessageAuthorDisplayName: string | null;
  attachments: IAttachment[];
  chatId: string;
  dateOfCreate: string;
}
