import IChatDto from "../../models/interfaces/IChatDto";
import IMessageDto from "../../models/interfaces/IMessageDto";


export interface IChatListWithMessagesDataItem {
  chat: IChatDto;
  messages: IMessageDto[];
}