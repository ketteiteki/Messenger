import { createAsyncThunk } from "@reduxjs/toolkit";
import UsersAPI from "../../services/api/UsersAPI";

export const getUserAsync = createAsyncThunk(
  "users/getUser",
  async (userId: string) => {
    const response = await UsersAPI.getUserAsync(userId);
    return response.data;
  }
);
