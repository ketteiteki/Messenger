import { createAsyncThunk } from "@reduxjs/toolkit";
import DialogAPI from "../../services/api/DialogsAPI";

interface IDelDeleteChatAsyncThuckAction {
    dialogId: string,
     isDeleteForAll: boolean
}

export const delDeleteDialogAsync = createAsyncThunk(
    "dialogs/delDeleteDialog",
    async ({dialogId, isDeleteForAll}: IDelDeleteChatAsyncThuckAction) => {
      const response = await DialogAPI.delDeleteDialogAsync(dialogId, isDeleteForAll);
      return response.data;
    }
  );