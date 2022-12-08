import { createAsyncThunk } from "@reduxjs/toolkit";
import MessagesAPI from "../../services/api/MessagesAPI";

interface IGetMessageListAsyncThunkAction {
  chatId: string;
  fromMessageTime: string | null;
  limit: number;
}

interface IPostCreateMessageAsync {
  text: string;
  chatId: string;
  replyToId: string | null;
  files: File[];
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

export const postCreateMessageAsync = createAsyncThunk(
  "messages/postCreateMessageAsync",
  async ({ text, chatId, replyToId, files }: IPostCreateMessageAsync) => {
    const response = await MessagesAPI.postCreateMessageAsync(
      text,
      chatId,
      replyToId,
      files
    );
    return response.data;
  }
);
