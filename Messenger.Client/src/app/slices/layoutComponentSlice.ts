import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { RootState } from "../store";

export interface ILayoutComponentState {
  data: {
    showProfile: boolean;
    showChatCreater: boolean;
    showChatInfo: boolean;
    showConfirmDeleteDialog: boolean;
    showBlackBackground: boolean;
  };
}

const initialState: ILayoutComponentState = {
  data: {
    showProfile: false,
    showChatCreater: false,
    showChatInfo: false,
    showConfirmDeleteDialog: false,
    showBlackBackground: false,
  },
};

export const layoutComponentSlice = createSlice({
  name: "layoutComponent",
  initialState,
  reducers: {
    setShowProfile: (state, action: PayloadAction<boolean>) => {
      state.data.showProfile = action.payload;
    },
    setShowChatCreater: (state, action: PayloadAction<boolean>) => {
      state.data.showChatCreater = action.payload;
    },
    setShowChatInfo: (state, action: PayloadAction<boolean>) => {
      state.data.showChatInfo = action.payload;
    },
    setShowBlackBackground: (state, action: PayloadAction<boolean>) => {
      state.data.showBlackBackground = action.payload;
    },
    setShowConfirmDeleteDialog: (state, action: PayloadAction<boolean>) => {
      state.data.showConfirmDeleteDialog = action.payload;
    },
  },
});

export const selectLayoutComponent = (state: RootState) => state.layoutComponent;

export const {
  setShowProfile,
  setShowChatCreater,
  setShowChatInfo,
  setShowBlackBackground,
  setShowConfirmDeleteDialog
} = layoutComponentSlice.actions;

export default layoutComponentSlice.reducer;
