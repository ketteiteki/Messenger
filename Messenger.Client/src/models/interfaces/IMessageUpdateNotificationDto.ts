export default interface IMessageUpdateNotificationDto {
  ownerId: string | null;
  chatId: string;
  messageId: string;
  updatedText: string;
  isLastMessage: boolean;
}
