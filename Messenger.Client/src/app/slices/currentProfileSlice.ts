import { createSlice } from "@reduxjs/toolkit";
import { RequestStatusEnum } from "../../models/enums/RequestStatusEnum";
import { RootState } from "../store";
import { IUser } from "../../models/interfaces/IUser";
import { getUserAsync } from "../thunks/usersThucks";

export interface IСurrentProfileState {
  data: IUser | null;
  status: RequestStatusEnum;
}

const initialState: IСurrentProfileState = {
  data: null,
  status: RequestStatusEnum.none,
};

export const currentProfileSlice = createSlice({
  name: "currentProfile",
  initialState,
  reducers: {
    setNullCurrentProfile: (state) => {
      state.data = null;
    },
  },
  extraReducers(builder) {
    builder
      .addCase(getUserAsync.pending, (state) => {
        state.status = RequestStatusEnum.loading;
      })
      .addCase(getUserAsync.fulfilled, (state, action) => {
        state.status = RequestStatusEnum.success;
        state.data = action.payload;
      })
      .addCase(getUserAsync.rejected, (state) => {
        state.status = RequestStatusEnum.fail;
        state.data = null;
      });
  },
});

export const { setNullCurrentProfile } = currentProfileSlice.actions;

export const selectCurrentProfile = (state: RootState) => state.currentProfile;

export default currentProfileSlice.reducer;
