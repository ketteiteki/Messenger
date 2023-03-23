export default interface IMessageDeleteNotificationDto {
  ownerId: string | null;
  chatId: string;
  messageId: string;
  newLastMessageId: string | null;
  newLastMessageText: string;
  newLastMessageAuthorDisplayName: string;
  newLastMessageDateOfCreate: string | null;
}
