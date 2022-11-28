import { CreateChatEnum } from "../../models/enums/CreateChatEnum";
import { IChat } from "../../models/interfaces/IChat";
import api from "./baseAPI";

export default class ChatsAPI {
  public static async getChatAsync(chatId: string) {
    return await api.get<IChat>(`/Chats/${chatId}`);
  }

  public static async getChatListAsync() {
    return await api.get<IChat[]>(`/Chats`);
  }

  public static async getChatListBySearchAsync(searchText: string) {
    const params = {
      searchText
    }

    return await api.get<IChat[]>(`/Chats/search`, {params});
  }

  public static async postJoinToChatAsync(chatId: string) {
    const params = {
      chatId,
    };

    return await api.post<IChat>(
      `/Chats/joinToChat`,
      null,
      { params }
    );
  }

  public static async postLeaveFromChatAsync(chatId: string) {
    const params = {
      chatId,
    };

    return await api.post<IChat>(
      `/Chats/leave`,
      null,
      { params }
    );
  }

  public static async postCreateChatAsync(
    name: string,
    title: string,
    type: CreateChatEnum,
    avatarFile: File | null
  ) {

    const data = {
      name,
      title,
      type,
      avatarFile
    }

    return await api.post<IChat>(
      `/Chats/createChat`,
      data,
      {headers: {
        'Content-Type': 'multipart/form-data'
      }}
    );
  }

  public static async putUpdateChatDataAsync(
    chatId: string,
    name: string,
    title: string
  ) {
    const data = {
      chatId,
      name,
      title,
    };
    
    return await api.put<IChat>(
      `/Chats/updateChatData`,
      data
    );
  }

  public static async putUpdateChatAvatarAsync(
    chatId: string,
    avatarFile: File
  ) {

    const data = {
      chatId,
      avatarFile
    }

    return await api.put<IChat>(
      `/Chats/updateChatAvatar`,
      data, 
      {headers: {
        'Content-Type': 'multipart/form-data'
      }}
    );
  }

  public static async delDeleteChatAsync(chatId: string) {
    const params = {
      chatId,
    };

    return await api.delete<IChat>(
      `/Chats/deleteChat`,
      { params }
    );
  }
}
