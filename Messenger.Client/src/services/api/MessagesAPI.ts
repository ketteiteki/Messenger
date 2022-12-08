import { IMessage } from "../../models/interfaces/IMessage";
import api from "./baseAPI";

export default class MessagesAPI {
  public static async getMessageListAsync(
    chatId: string,
    fromMessageDateTime: string | null,
    limit: number
  ) {
    const params = {
      chatId,
      fromMessageDateTime,
      limit,
    };

    return await api.get<IMessage[]>(`/Messages`, { params });
  }

  public static async getMessageListBySearchAsync(
    chatId: string,
    fromMessageDateTime: Date | null,
    limit: number,
    searchText: string
  ) {
    const params = {
      chatId,
      fromMessageDateTime,
      limit,
      searchText,
    };

    return await api.get<IMessage[]>(`/Messages/search`, { params });
  }

  public static async postCreateMessageAsync(
    text: string,
    chatId: string,
    replyToId: string | null,
    files: File[]
  ) {
    const data = {
      text,
      chatId,
      replyToId,
      files,
    };

    const headers = {
      "Content-Type": "multipart/form-data",
    };

    return await api.post<IMessage>(`/Messages/createMessage`, data, {
      headers,
    });
  }

  public static async putUpdateMessageAsync(id: string, text: string) {
    const data = {
      id,
      text,
    };

    return api.put<IMessage>(`/Messages/updateMessage`, data);
  }

  public static async delDeleteMessageAsync(
    messageId: string,
    isDeleteForAll: boolean
  ) {
    const params = {
      isDeleteForAll,
    };

    return api.delete<IMessage>(`/Messages/${messageId}`, { params });
  }
}
