import chatListReducer from './slices/chatListSlice';
import { configureStore, ThunkAction, Action } from '@reduxjs/toolkit';
import authorizationReducer from './slices/authorizationSlice';
import counterReducer from './slices/counterSlice';
import currentProfileReducer from './slices/currentProfileSlice';
import currentChatReducer from './slices/currentChatSlice';
import layoutComponentReducer from './slices/layoutComponentSlice';
import sessionReducer from './slices/sessionSlice';

export const store = configureStore({
  reducer: {
    counter: counterReducer,
    authorization: authorizationReducer,
    chatList: chatListReducer,
    currentProfile: currentProfileReducer,
    currentChat: currentChatReducer,
    layoutComponent: layoutComponentReducer,
    session: sessionReducer
  },
});

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;
export type AppThunk<ReturnType = void> = ThunkAction<
  ReturnType,
  RootState,
  unknown,
  Action<string>
>;
