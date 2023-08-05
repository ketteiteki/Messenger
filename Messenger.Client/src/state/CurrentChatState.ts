import { makeAutoObservable, runInAction } from "mobx";
import IChatDto from "../models/interfaces/IChatDto";
import IMessageDto from "../models/interfaces/IMessageDto";
import ChatApi from "../services/api/ChatApi";
import ConversationApi from "../services/api/ConversationApi";
import UsersApi from "../services/api/UserApi";

class CurrentChatState {
  public chat: IChatDto | null = null;
  public messages: IMessageDto[] = [];

  constructor() {
    makeAutoObservable(
      this,
      {},
      {
        deep: true,
      }
    );
  }

  public setChatAndMessages = (chat: IChatDto, messages: IMessageDto[]) => {
    this.chat = chat;
    this.messages = messages;
  };

  public setChatAndMessagesNull = () => {
    this.chat = null;
    this.messages = [];
  };

  public updateChatByChat = (chat: IChatDto) => {
    if (this.chat) {
      this.chat.title = chat.title;
      this.chat.name = chat.name;
    }
  };

  public setMemberListPage(value: number) {
    if (this.chat) {
      this.chat.memberListPage = value;
    }
  }

  public clearChatAndMessages() {
    this.chat = null;
    this.messages = [];
  };

  //api
  public banUserAsync = async (userId: string, banMinutes: number) => {
    if (!this.chat) return;
    
    this.chat.members = this.chat.members.filter(x => x.id !== userId);

    const response = await ConversationApi.postConversationBanUserAsync(
      this.chat.id,
      userId,
      banMinutes
    );

    return response;
  };

  public kickUserAsync = async (userId: string) => {
    if (!this.chat) return;
    
    this.chat.members = this.chat.members.filter(x => x.id !== userId);

    const response = await ConversationApi.postConversationRemoveUserAsync(
      this.chat.id,
      userId);

    return response;
  };

  public getUserListByChatAsync = async (
    chatId: string,
    limit: number,
    page: number
  ) => {
    const response = await UsersApi.getUserListByChatAsync(chatId, limit, page);

    if (response.status === 200) {
      runInAction(() => {
        this.chat?.members.push(...response.data);
      });
    }

    return response;
  };

  public postJoinToChatAsync = async (chatId: string) => {
    const response = await ChatApi.postJoinToChatAsync(chatId);

    if (response.status === 200 && this.chat) {
      this.chat.isMember = true;
    }
  }
}

export const currentChatState = new CurrentChatState();
