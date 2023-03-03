import { ChatType } from '../enum/ChatType';
import IRoleUserByChatDto from './IRoleUserByChatDto';
import IUserDto from './IUserDto';


export default interface IChatDto {
    id: string;
    name: string | null;
    title: string | null;
    type: ChatType;
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
    roleUser: IRoleUserByChatDto;
    members: IUserDto[];
    usersWithRole: IRoleUserByChatDto[];
}