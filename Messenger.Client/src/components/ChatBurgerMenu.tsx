import React from "react";
import { useAppDispatch, useAppSelector } from "../app/hooks";
import {
  selectCurrentChat,
  setCurrentChatData,
} from "../app/slices/currentChatSlice";
import {
  setShowBlackBackground,
  setShowChatInfo,
  setShowConfirmDeleteDialog,
} from "../app/slices/layoutComponentSlice";
import {
  delDeleteChatAsync,
  postLeaveFromChatAsync,
} from "../app/thunks/chatsThuck";

interface IChatBurgerMenuProps {
  setShowBurgerMenu: React.Dispatch<React.SetStateAction<boolean>>;
  isDialog: boolean;
}

export const ChatBurgerMenu = ({
  setShowBurgerMenu,
  isDialog,
}: IChatBurgerMenuProps) => {
  const currentChatState = useAppSelector(selectCurrentChat);
  const dispatch = useAppDispatch();

  const onClickDeleteChat = () => {
    if (isDialog) {
      dispatch(setShowBlackBackground(true));
      dispatch(setShowConfirmDeleteDialog(true));
      return;
    }

    if (!currentChatState.data) return;

    dispatch(delDeleteChatAsync(currentChatState.data.id));
    dispatch(setCurrentChatData(null));
  };

  const onClickLeaveFromChat = () => {
    if (currentChatState.data) {
      dispatch(postLeaveFromChatAsync(currentChatState.data.id));
      dispatch(setCurrentChatData(null));
    }
  };

  const onClickVeiwChatInfo = () => {
    dispatch(setShowBlackBackground(true));
    dispatch(setShowChatInfo(true));
  };

  return (
    <div
      className="chat-burger-menu"
      onMouseLeave={() => setShowBurgerMenu(false)}
    >
      {!isDialog && (
        <button
          className="chat-burger-menu__view-chat-info-button"
          onClick={onClickVeiwChatInfo}
        >
          View chat info
        </button>
      )}
      {(isDialog || currentChatState.data?.isOwner) &&
        currentChatState.data?.isMember && (
          <button
            className="chat-burger-menu__delete-button"
            onClick={onClickDeleteChat}
          >
            Delete
          </button>
        )}
      {!isDialog && currentChatState.data?.isMember && (
        <button
          className="chat-burger-menu__leave-button"
          onClick={onClickLeaveFromChat}
        >
          Leave
        </button>
      )}
    </div>
  );
};
