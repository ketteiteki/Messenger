

export default interface IPermissionDto {
  chatId: string;
  userId: string;
  canSendMedia: boolean;
  muteDateOfExpire: string | null;
}