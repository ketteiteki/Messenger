import IAttachmentDto from "./IAttachmentDto";


export default interface IMessageDto {
    id: string;
    text: string;
    isEdit: boolean;
    ownerId: string | null;
    ownerDisplayName: string | null;
    ownerAvatarLink: string | null;
    replyToMessageId: string | null;
    replyToMessageText: string | null;
    replyToMessageAuthorDisplayName: string | null;
    attachments: IAttachmentDto[];
    chatId: string;
    dateOfCreate: string;

    loading: boolean;
    isMessageRealtime: boolean;
}