import { createAsyncThunk } from "@reduxjs/toolkit";
import MessagesAPI from "../../services/api/MessagesAPI";

interface IGetMessageListAsyncThunkAction {
  chatId: string;
  fromMessageTime: Date | null;
  limit: number;
}

export const getMessageListAsync = createAsyncThunk(
  "messages/getMessageList",
  async ({
    chatId,
    fromMessageTime,
    limit,
  }: IGetMessageListAsyncThunkAction) => {
    const response = await MessagesAPI.getMessageListAsync(
      chatId,
      fromMessageTime,
      limit
    );
    return response.data;
  }
);
