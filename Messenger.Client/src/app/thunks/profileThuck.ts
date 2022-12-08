import { createAsyncThunk } from "@reduxjs/toolkit";
import ProfileAPI from "../../services/api/ProfileAPI";

export interface IPutUpdateProfileAsyncThunkAction {
    displayName: string,
    nickName: string,
    bio: string
}

export const putUpdateProfileAsync = createAsyncThunk(
    "profile/putUpdateProfile",
    async (arg: IPutUpdateProfileAsyncThunkAction) => {
      const response = await ProfileAPI.putUpdateProfileAsync(arg.displayName, arg.nickName, arg.bio);
      return response.data;
    }
  );
  