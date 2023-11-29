import IChatDto from "../../models/interfaces/IChatDto";
import api from "./baseAPI";

export default class DialogApi {
    public static async getDialogAsync(userId: string) {
      return await api.get<IChatDto>(`/Dialogs/${userId}`);
    }
  
    public static async postCreateDialogAsync(userId: string) {
      const params = {
        userId,
      };
  
      return await api.post<IChatDto>(`/Dialogs/createDialog`, null, { params });
    }
  
    public static async delDeleteDialogAsync(
      dialogId: string,
      isDeleteForAll: boolean
    ) {
      const params = {
        dialogId,
        isDeleteForAll,
      };
  
      return await api.delete<IChatDto>(`/Dialogs/deleteDialog`, { params });
    }
  }