import { createSlice } from "@reduxjs/toolkit";
import { RequestStatusEnum } from "../../models/enums/RequestStatusEnum";
import { ISession } from "../../models/interfaces/ISession";
import { RootState } from "../store";
import { delDeleteSessionAsync, getSessionListAsync } from "../thunks/authorizationThucks";

interface IAuthorization {
  accessToken: string | null;
  refreshToken: string | null;
  id: string | null;
  displayName: string | null;
  nickName: string | null;
  bio: string | null;
  avatarLink: string | null;
}

export interface ISessionState {
  data: ISession[];
  status: RequestStatusEnum;
}

const initialState: ISessionState = {
  data: [],
  status: RequestStatusEnum.none,
};

export const sessionSlice = createSlice({
  name: "session",
  initialState,
  reducers: {
    removeAllSessionLocal: (state) => {
        state.data = [];
    }
  },
  extraReducers(builder) {
      builder
      .addCase(getSessionListAsync.fulfilled, (state, action) => {
        state.data.push(...action.payload);
        state.status = RequestStatusEnum.success
      })
      .addCase(getSessionListAsync.rejected, (state) => {
        state.status = RequestStatusEnum.fail
      })

      builder
        .addCase(delDeleteSessionAsync.fulfilled, (state, action) => {
          state.data = state.data.filter(s => s.id !== action.payload.id);
        })
  },
});

export const { removeAllSessionLocal } = sessionSlice.actions;

export const selectSession = (state: RootState) => state.session;

export default sessionSlice.reducer;
