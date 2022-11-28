import { createSlice } from "@reduxjs/toolkit";
import { RequestStatusEnum } from "../../models/enums/RequestStatusEnum";
import TokenService from "../../services/messenger/TokenService";
import { RootState } from "../store";
import {
  registrationAsync,
  loginAsync,
  authorizationAsync,
} from "../thunks/authorizationThucks";
import { putUpdateProfileAsync } from "../thunks/profileThuck";

interface IAuthorization {
  accessToken: string | null;
  refreshToken: string | null;
  id: string | null;
  displayName: string | null;
  nickName: string | null;
  bio: string | null;
  avatarLink: string | null;
}

export interface IAuthorizationState {
  data: IAuthorization | null;
  status: RequestStatusEnum;
}

const initialState: IAuthorizationState = {
  data: null,
  status: RequestStatusEnum.none,
};

export const authorizationSlice = createSlice({
  name: "authorization",
  initialState,
  reducers: {
    clearAuthorizationData: (state) => {
      state.data = null;
      state.status = RequestStatusEnum.none;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(registrationAsync.pending, (state) => {
        state.status = RequestStatusEnum.loading;
      })
      .addCase(registrationAsync.fulfilled, (state, action) => {
        state.status = RequestStatusEnum.success;
        state.data = action.payload;
        TokenService.setLocalAccessToken(action.payload.accessToken);
        TokenService.setLocalRefreshToken(action.payload.refreshToken);
      })
      .addCase(registrationAsync.rejected, (state) => {
        state.status = RequestStatusEnum.fail;
      });

    builder
      .addCase(loginAsync.pending, (state) => {
        state.status = RequestStatusEnum.loading;
      })
      .addCase(loginAsync.fulfilled, (state, action) => {
        state.status = RequestStatusEnum.success;
        state.data = action.payload;
        TokenService.setLocalAccessToken(action.payload.accessToken);
        TokenService.setLocalRefreshToken(action.payload.refreshToken);
      })
      .addCase(loginAsync.rejected, (state) => {
        state.status = RequestStatusEnum.fail;
      });

    builder
      .addCase(authorizationAsync.pending, (state) => {
        state.status = RequestStatusEnum.loading;
      })
      .addCase(authorizationAsync.fulfilled, (state, action) => {
        state.status = RequestStatusEnum.success;
        state.data = action.payload;
        TokenService.setLocalAccessToken(action.payload.accessToken);
        TokenService.setLocalRefreshToken(action.payload.refreshToken);
      })
      .addCase(authorizationAsync.rejected, (state) => {
        state.status = RequestStatusEnum.fail;
      });

    builder
      .addCase(putUpdateProfileAsync.pending, (state) => {
        state.status = RequestStatusEnum.loading;
      })
      .addCase(putUpdateProfileAsync.fulfilled, (state, action) => {
        state.status = RequestStatusEnum.success;

        if (state.data != null) {
          state.data.displayName = action.payload.displayName;
          state.data.nickName = action.payload.nickname;
          state.data.bio = action.payload.bio;
        }
      })
      .addCase(putUpdateProfileAsync.rejected, (state) => {
        state.status = RequestStatusEnum.fail;
      });
  },
});

export const { clearAuthorizationData } = authorizationSlice.actions;

export const selectAuthorization = (state: RootState) => state.authorization;

export default authorizationSlice.reducer;
