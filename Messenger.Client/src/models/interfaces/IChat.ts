import { IRoleUserByChat } from "./IRoleUserByChat";
import { IUser } from "./IUser";

export interface IChat {
  id: string;
  name: string | null;
  title: string | null;
  type: number;
  avatarLink: string | null;
  lastMessageId: string | null;
  lastMessageText: string | null;
  lastMessageAuthorDisplayName: string | null;
  lastMessageDateOfCreate: string | null;
  membersCount: number;
  canSendMedia: boolean;
  isOwner: boolean;
  isMember: boolean;
  muteDateOfExpire: string | null;
  banDateOfExpire: string | null;
  roleUser: IRoleUserByChat | null;
  members: IUser[] | null;
}
