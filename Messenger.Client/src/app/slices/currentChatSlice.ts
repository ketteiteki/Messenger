import { IMessage } from './../../models/interfaces/IMessage';
import {
  putUpdateChatDataAsync,
  postJoinToChatAsync,
} from "./../thunks/chatsThuck";
import { IChat } from "../../models/interfaces/IChat";
import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { RequestStatusEnum } from "../../models/enums/RequestStatusEnum";
import { RootState } from "../store";
import {
  getMessageListAsync,
  postCreateMessageAsync,
} from "../thunks/messageThunks";
import MessageEntity from "../../models/MessageEntity";
import { IChatListDataItem } from "./chatListSlice";

export interface IСurrentChatState {
  data: IChat | null;
  messages: IMessage[];
  status: RequestStatusEnum;
}

const initialState: IСurrentChatState = {
  data: null,
  messages: [],
  status: RequestStatusEnum.none,
};

export const currentChatSlice = createSlice({
  name: "currentChat",
  initialState,
  reducers: {
    setCurrentChatData: (
      state,
      action: PayloadAction<IChatListDataItem | null>
    ) => {
      if (action.payload === null) {
        state.data = null;
        state.messages = [];

        return;
      }

      state.data = action.payload.chat;
      state.messages = action.payload.messages;
    },
    setNullMessageData: (state) => {
      state.messages = [];
    },
    addMessageCurrentChat: (state, action: PayloadAction<MessageEntity | IMessage>) => {
      state.messages.push(action.payload);
    },
    updateMessageIdAfterCreateMessage: (
      state,
      action: PayloadAction<{ lastMessageId: string; message: IMessage }>
    ) => {
      const indexMessageById = state.messages.findIndex(
        (m) => m.id === action.payload.lastMessageId
      );

      if (indexMessageById) {
        state.messages[indexMessageById] = action.payload.message;
      }
    },
  },
  extraReducers(builder) {
    builder
      .addCase(putUpdateChatDataAsync.fulfilled, (state, action) => {
        if (state.data) {
          state.data.name = action.payload.name;
          state.data.title = action.payload.title;
          state.status = RequestStatusEnum.success;
        }
      })
      .addCase(putUpdateChatDataAsync.rejected, (state) => {
        state.status = RequestStatusEnum.fail;
      });

    builder.addCase(getMessageListAsync.fulfilled, (state, action) => {
      state.messages.unshift(...action.payload);
    });

    builder.addCase(postJoinToChatAsync.fulfilled, (state, action) => {
      state.data = action.payload;
    });
  },
});

export const currentChatSliceActions =
  currentChatSlice.actions;

export const selectCurrentChat = (state: RootState) => state.currentChat;

export default currentChatSlice.reducer;
