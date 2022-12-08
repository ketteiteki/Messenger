import { getMessageListAsync } from "./../thunks/messageThunks";
import { IMessage } from "./../../models/interfaces/IMessage";
import {
  delDeleteChatAsync,
  postJoinToChatAsync,
  postLeaveFromChatAsync,
  putUpdateChatDataAsync,
} from "./../thunks/chatsThuck";
import { IChat } from "./../../models/interfaces/IChat";
import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { RequestStatusEnum } from "../../models/enums/RequestStatusEnum";
import { RootState } from "../store";
import {
  getChatListAsync,
  getChatListBySearchAsync,
  postCreateChatAsync,
} from "../thunks/chatsThuck";
import { delDeleteDialogAsync } from "../thunks/dialogsThuck";
import MessageEntity from "../../models/MessageEntity";

export interface IChatListDataItem {
  chat: IChat;
  messages: IMessage[];
}

export interface IChatListState {
  data: IChatListDataItem[];
  status: RequestStatusEnum;
}

const initialState: IChatListState = {
  data: [],
  status: RequestStatusEnum.none,
};

export const chatListSlice = createSlice({
  name: "chatList",
  initialState,
  reducers: {
    setLastMessage: (state, action: PayloadAction<MessageEntity | IMessage>) => {
      const dataItem = state.data.find(c => c.chat.id === action.payload.chatId);

      if (dataItem === undefined) return;

      dataItem.chat.lastMessageId = action.payload.id;
      dataItem.chat.lastMessageText = action.payload.text;
      dataItem.chat.lastMessageAuthorDisplayName = action.payload.ownerDisplayName;
      dataItem.chat.lastMessageDateOfCreate = action.payload.dateOfCreate;
    },
    addMessageInChatOfChatList: (state, action: PayloadAction<MessageEntity | IMessage>) => {
      const dataItem = state.data.find(c => c.chat.id === action.payload.chatId);

      if (dataItem === undefined) return;

      dataItem?.messages.push(action.payload);
    },
    updateMessageIdAfterCreateMessage: (
      state,
      action: PayloadAction<{ lastMessageId: string; message: IMessage }>
    ) => {
      const dataItem = state.data.find(d => d.chat.id === action.payload.message.chatId);

      if (!dataItem) return;

      const indexMessageById = dataItem.messages.findIndex(
        (m) => m.id === action.payload.lastMessageId
      );

      if (indexMessageById) {
        dataItem.messages[indexMessageById] = action.payload.message;
      }
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(getChatListAsync.pending, (state) => {
        state.status = RequestStatusEnum.loading;
      })
      .addCase(getChatListAsync.fulfilled, (state, action) => {
        state.status = RequestStatusEnum.success;
        action.payload.forEach((c) => {
          state.data.push({ chat: c, messages: [] });
        });
      })
      .addCase(getChatListAsync.rejected, (state) => {
        state.status = RequestStatusEnum.fail;
      });

    builder
      .addCase(getChatListBySearchAsync.pending, (state) => {
        state.status = RequestStatusEnum.loading;
      })
      .addCase(getChatListBySearchAsync.fulfilled, (state, action) => {
        state.status = RequestStatusEnum.success;
        action.payload.forEach((c) => {
          state.data.push({ chat: c, messages: [] });
        });
      })
      .addCase(getChatListBySearchAsync.rejected, (state) => {
        state.status = RequestStatusEnum.fail;
      });

    builder
      .addCase(postCreateChatAsync.fulfilled, (state, action) => {
        state.status = RequestStatusEnum.success;
        state.data.push({ chat: action.payload, messages: [] });
      })
      .addCase(postCreateChatAsync.rejected, (state) => {
        state.status = RequestStatusEnum.fail;
      });

    builder.addCase(putUpdateChatDataAsync.fulfilled, (state, action) => {
      state.data.map((i) => {
        if (i.chat.id === action.payload.id) {
          i.chat.title = action.payload.title;
          i.chat.name = action.payload.name;
          return { chat: i.chat, messages: i.messages };
        }
        return { chat: i.chat, messages: i.messages };
      });
    });

    builder
      .addCase(postLeaveFromChatAsync.fulfilled, (state, action) => {
        state.data = state.data.filter((i) => i.chat.id !== action.payload.id);
      })
      .addCase(delDeleteChatAsync.fulfilled, (state, action) => {
        state.data = state.data.filter((i) => i.chat.id !== action.payload.id);
      })
      .addCase(delDeleteDialogAsync.fulfilled, (state, action) => {
        state.data = state.data.filter((i) => i.chat.id !== action.payload.id);
      });

    builder.addCase(postJoinToChatAsync.fulfilled, (state, action) => {
      state.data = state.data.filter((i) => i.chat.id !== action.payload.id);
      state.data.push({ chat: action.payload, messages: [] });
    });

    builder.addCase(getMessageListAsync.fulfilled, (state, action) => {
      const dataItem = state.data.find(
        (c) => c.chat.id === action.payload[0].chatId
      );

      if (dataItem === undefined) return;

      const arrayMessages = action.payload.reverse();
      
      dataItem.messages.push(...arrayMessages);
    });
  },
});

export const chatListSliceActions = chatListSlice.actions;

export const selectChatList = (state: RootState) => state.chatList;

export default chatListSlice.reducer;
