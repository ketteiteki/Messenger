import { CreateChatEnum } from "../../models/enum/CreateChat";
import IChatDto from "../../models/interfaces/IChatDto";
import api from "./baseApi";


export default class ChatApi {
  public static async getChatAsync(chatId: string) {
    return await api.get<IChatDto>(`/Chats/${chatId}`);
  }

  public static async getChatListAsync() {
    return await api.get<IChatDto[]>(`/Chats`);
  }

  public static async getChatListBySearchAsync(searchText: string) {
    const params = {
      searchText
    }

    return await api.get<IChatDto[]>(`/Chats/search`, {params});
  }

  public static async postJoinToChatAsync(chatId: string) {
    const params = {
      chatId,
    };

    return await api.post<IChatDto>(
      `/Chats/joinToChat`,
      null,
      { params }
    );
  }

  public static async postLeaveFromChatAsync(chatId: string) {
    const params = {
      chatId,
    };

    return await api.post<IChatDto>(
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

    return await api.post<IChatDto>(
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
    
    return await api.put<IChatDto>(
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

    return await api.put<IChatDto>(
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

    return await api.delete<IChatDto>(
      `/Chats/deleteChat`,
      { params }
    );
  }
}