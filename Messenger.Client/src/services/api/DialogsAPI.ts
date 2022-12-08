import { IChat } from "../../models/interfaces/IChat";
import api from "./baseAPI";

export default class DialogAPI {
  public static async getDialogAsync(userId: string) {
    return await api.get<IChat>(`/Dialogs/${userId}`);
  }

  public static async postCreateDialogAsync(userId: string) {
    const params = {
      userId,
    };

    return await api.post<IChat>(`/Dialogs/createDialog`, null, { params });
  }

  public static async delDeleteDialogAsync(
    dialogId: string,
    isDeleteForAll: boolean
  ) {
    const params = {
      dialogId,
      isDeleteForAll,
    };

    return await api.delete<IChat>(`/Dialogs/deleteDialog`, { params });
  }
}