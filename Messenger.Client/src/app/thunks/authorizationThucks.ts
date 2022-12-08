import AuthAPI from "../../services/api/AuthAPI";
import { createAsyncThunk } from "@reduxjs/toolkit";

interface IRegistrationThunkAction {
  displayName: string;
  nickname: string;
  password: string;
}

interface ILoginThunkAction {
  nickname: string;
  password: string;
}

interface IAuthorizationThunkAction {
  accessToken: string;
}

export const registrationAsync = createAsyncThunk(
  "authorization/registration",
  async (action: IRegistrationThunkAction) => {
    const response = await AuthAPI.postRegistrationAsync(
      action.displayName,
      action.nickname,
      action.password
    );
    return response.data;
  }
);

export const loginAsync = createAsyncThunk(
  "authorization/login",
  async (action: ILoginThunkAction) => {
    const response = await AuthAPI.postLoginAsync(action.nickname, action.password);
    return response.data;
  }
);

export const authorizationAsync = createAsyncThunk(
  "authorization/authorization",
  async (action: IAuthorizationThunkAction) => {
    const response = await AuthAPI.getAuthorizationAsync(action.accessToken);
    return response.data;
  }
);

export const getSessionListAsync = createAsyncThunk(
  "authorization/getSessionList",
  async () => {
    const response = await AuthAPI.getSessionListAsync();
    return response.data;
  }
);

export const delDeleteSessionAsync = createAsyncThunk(
  "authorization/delDeleteSession",
  async (sessionId: string) => {
    const response = await AuthAPI.delDeleteSessionAsync(sessionId);
    return response.data;
  }
);

