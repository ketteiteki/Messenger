import { CreateChatEnum } from './../../models/enums/CreateChatEnum';
import ChatsAPI from '../../services/api/ChatsAPI';
import { createAsyncThunk } from "@reduxjs/toolkit";

interface IPostCreateChatAsyncThunkAction {
  name: string, 
  title: string, 
  type: CreateChatEnum, 
  avatarFile: File | null
}

interface IPutUpdateChatDataThunkAction {
  chatId: string,
  name: string,
  title: string
}

export const getChatListAsync = createAsyncThunk(
    "chats/getChatList",
    async () => {
      const response = await ChatsAPI.getChatListAsync();
      return response.data;
    }
  );

export const getChatListBySearchAsync = createAsyncThunk(
  "chats/getChatListBySearch",
  async (searchText: string) => {
    const response = await ChatsAPI.getChatListBySearchAsync(searchText);
    return response.data;
  }
);

export const postCreateChatAsync = createAsyncThunk(
  "chats/postCreateChat",
  async ({name, title, type, avatarFile}: IPostCreateChatAsyncThunkAction) => {
    const response = await ChatsAPI.postCreateChatAsync(name, title, type, avatarFile);
    return response.data;
  }
);

export const putUpdateChatDataAsync = createAsyncThunk(
  "chats/putUpdateChatData",
  async ({chatId, name, title}: IPutUpdateChatDataThunkAction) => {
    const response = await ChatsAPI.putUpdateChatDataAsync(chatId, name, title);
    return response.data;
  }
);

export const postLeaveFromChatAsync = createAsyncThunk(
  "chats/postLeaveFromChat",
  async (chatId: string) => {
    const response = await ChatsAPI.postLeaveFromChatAsync(chatId);
    return response.data;
  }
);

export const delDeleteChatAsync = createAsyncThunk(
  "chats/delDeleteChat",
  async (chatId: string) => {
    const response = await ChatsAPI.delDeleteChatAsync(chatId);
    return response.data;
  }
);

export const postJoinToChatAsync = createAsyncThunk(
  "chats/postJoinToChat",
  async (chatId: string) => {
    const response = await ChatsAPI.postJoinToChatAsync(chatId);
    return response.data;
  }
);