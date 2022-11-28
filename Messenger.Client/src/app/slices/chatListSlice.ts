import {
  delDeleteChatAsync,
  postJoinToChatAsync,
  postLeaveFromChatAsync,
  putUpdateChatDataAsync,
} from "./../thunks/chatsThuck";
import { IChat } from "./../../models/interfaces/IChat";
import { createSlice } from "@reduxjs/toolkit";
import { RequestStatusEnum } from "../../models/enums/RequestStatusEnum";
import { RootState } from "../store";
import {
  getChatListAsync,
  getChatListBySearchAsync,
  postCreateChatAsync,
} from "../thunks/chatsThuck";
import { delDeleteDialogAsync } from "../thunks/dialogsThuck";

export interface IChatListState {
  data: IChat[];
  status: RequestStatusEnum;
}

const initialState: IChatListState = {
  data: [],
  status: RequestStatusEnum.none,
};

export const chatListSlice = createSlice({
  name: "chatList",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(getChatListAsync.pending, (state) => {
        state.status = RequestStatusEnum.loading;
      })
      .addCase(getChatListAsync.fulfilled, (state, action) => {
        state.status = RequestStatusEnum.success;
        state.data = action.payload;
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
        state.data = action.payload;
      })
      .addCase(getChatListBySearchAsync.rejected, (state) => {
        state.status = RequestStatusEnum.fail;
      });

    builder
      .addCase(postCreateChatAsync.fulfilled, (state, action) => {
        state.status = RequestStatusEnum.success;
        state.data.push(action.payload);
      })
      .addCase(postCreateChatAsync.rejected, (state) => {
        state.status = RequestStatusEnum.fail;
      });

    builder.addCase(putUpdateChatDataAsync.fulfilled, (state, action) => {
      state.data.map((c) => {
        if (c.id === action.payload.id) {
          c.title = action.payload.title;
          c.name = action.payload.name;
          return c;
        }
        return c;
      });
    });

    builder
      .addCase(postLeaveFromChatAsync.fulfilled, (state, action) => {
        state.data = state.data.filter((c) => c.id != action.payload.id);
      })
      .addCase(delDeleteChatAsync.fulfilled, (state, action) => {
        state.data = state.data.filter((c) => c.id != action.payload.id);
      })
      .addCase(delDeleteDialogAsync.fulfilled, (state, action) => {
        state.data = state.data.filter((c) => c.id != action.payload.id);
      });

    builder.addCase(postJoinToChatAsync.fulfilled, (state, action) => {
      state.data = state.data.filter(c => c.id !== action.payload.id);
      state.data.push(action.payload);
    });
  },
});

export const selectChatList = (state: RootState) => state.chatList;

export default chatListSlice.reducer;
