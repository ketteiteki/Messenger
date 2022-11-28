import {
  putUpdateChatDataAsync,
  postJoinToChatAsync,
} from "./../thunks/chatsThuck";
import { IChat } from "../../models/interfaces/IChat";
import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { RequestStatusEnum } from "../../models/enums/RequestStatusEnum";
import { RootState } from "../store";
import { IMessage } from "../../models/interfaces/IMessage";
import { getMessageListAsync } from "../thunks/MessageThunks";

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
    setCurrentChatData: (state, action: PayloadAction<IChat | null>) => {
      state.data = action.payload;
    },
    setNullMessageData: (state) => {
      state.messages = [];
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
      .addCase(putUpdateChatDataAsync.rejected, (state, action) => {
        state.status = RequestStatusEnum.fail;
      });

    builder.addCase(getMessageListAsync.fulfilled, (state, action) => {
      state.messages.push(...action.payload);
    });

    builder.addCase(postJoinToChatAsync.fulfilled, (state, action) => {
      state.data = action.payload;
      // state.data.membersCount++;
    });
  },
});

export const { setCurrentChatData, setNullMessageData } =
  currentChatSlice.actions;

export const selectCurrentChat = (state: RootState) => state.currentChat;

export default currentChatSlice.reducer;
