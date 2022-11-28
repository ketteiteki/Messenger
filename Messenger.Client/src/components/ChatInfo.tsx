import React, { useState } from "react";
import { useAppDispatch, useAppSelector } from "../app/hooks";
import { selectAuthorization } from "../app/slices/authorizationSlice";
import { selectCurrentChat } from "../app/slices/currentChatSlice";
import {
  postLeaveFromChatAsync,
  putUpdateChatDataAsync,
} from "../app/thunks/chatsThuck";
import { ChatTypeEnum } from "../models/enums/ChatTypeEnum";


export const ChatInfo = () => {
  const authorizationState = useAppSelector(selectAuthorization);
  const currentChatState = useAppSelector(selectCurrentChat);
  const dispatch = useAppDispatch();

  // const definedChatTitle =
  //   currentChatState.data?.title ||
  //   currentChatState.data?.members?.find(
  //     (m) => m.id !== authorizationState.data?.id
  //   )?.displayName;

  // const definedChatName =
  //   currentChatState.data?.name ||
  //   currentChatState.data?.members?.find(
  //     (m) => m.id !== authorizationState.data?.id
  //   )?.nickname;

  const [chatTitle, setChatTitle] = useState<string>(currentChatState.data?.title || "");
  const [chatName, setChatName] = useState<string>(currentChatState.data?.name || "");

  const onClickChangeChatData = () => {
    if (currentChatState.data) {
      dispatch(
        putUpdateChatDataAsync({
          chatId: currentChatState.data.id,
          name: chatName,
          title: chatTitle,
        })
      );
    }
  };

  // const onClickLeaveFromChat = async () => {
  //   if (currentChatState.data) {
  //     await dispatch(postLeaveFromChatAsync(currentChatState.data.id));
  //     setShowChatInfo(false);
  //     setShowBlackBackground(false);
  //   }
  // };

  const onClickDeleteDialog = async () => {};

  return (
    <div className="chat-info">
      <img
        className="chat-info__avatar"
        src={
          currentChatState.data?.avatarLink ||
          "https://i.pinimg.com/564x/d9/eb/32/d9eb32e507d11b4e5de6bcab602d3c66.jpg"
        }
        alt=""
      />
      <input
        disabled={!currentChatState.data?.isOwner}
        className="chat-info__chat-title"
        value={chatTitle}
        onChange={(e) => setChatTitle(e.currentTarget.value)}
      />
      <input
        disabled={!currentChatState.data?.isOwner}
        className="chat-info__chat-name"
        value={chatName}
        onChange={(e) => setChatName(e.currentTarget.value)}
      />
      {currentChatState.data?.isOwner &&
        currentChatState.data?.type != ChatTypeEnum.dialog && (
          <button
            className="chat-info__update-button"
            onClick={onClickChangeChatData}
          >
            Change
          </button>
        )}
      {/* {currentChatState.data?.type != ChatTypeEnum.dialog && (
        <button
          className="chat-info__leave-button"
          onClick={onClickLeaveFromChat}
        >
          Leave
        </button>
      )}
      {currentChatState.data?.type == ChatTypeEnum.dialog && (
        <button
          className="chat-info__delete-dialog-button"
          onClick={onClickLeaveFromChat}
        >
          Delete dialog
        </button>
      )} */}

      <div className="chat-info__user-list">
        {currentChatState.data?.type !== ChatTypeEnum.dialog && (
          <>
            <div className="chat-info__user-list__item">
              <img
                className="chat-info__user-list__item__avatar"
                src="https://i.pinimg.com/564x/d9/eb/32/d9eb32e507d11b4e5de6bcab602d3c66.jpg"
                alt=""
              />
              <div className="chat-info__user-list__item__user-date">
                <p className="chat-info__user-list__item__user-date__display-name">
                  display name
                </p>
                <p className="chat-info__user-list__item__user-date__nickname">
                  nickname
                </p>
              </div>
            </div>
            <div className="chat-info__user-list__item">
              <img
                className="chat-info__user-list__item__avatar"
                src="https://i.pinimg.com/564x/d9/eb/32/d9eb32e507d11b4e5de6bcab602d3c66.jpg"
                alt=""
              />
              <div className="chat-info__user-list__item__user-date">
                <p className="chat-info__user-list__item__user-date__display-name">
                  display name
                </p>
                <p className="chat-info__user-list__item__user-date__nickname">
                  nickname
                </p>
              </div>
            </div>
          </>
        )}
      </div>
    </div>
  );
};
