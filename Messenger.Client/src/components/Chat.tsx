import React, { useEffect, useState } from "react";
import TextArea from "rc-textarea";
import { ReactComponent as SendMessageSVG } from "../assets/svg/send-message.svg";
import { Message } from "./Message";
import { useAppDispatch, useAppSelector } from "../app/hooks";
import {
  selectCurrentChat,
  setNullMessageData,
} from "../app/slices/currentChatSlice";
import { selectAuthorization } from "../app/slices/authorizationSlice";
import { ChatBurgerMenu } from "./ChatBurgerMenu";
import { ChatTypeEnum } from "../models/enums/ChatTypeEnum";
import {
  setShowBlackBackground,
  setShowChatInfo,
  setShowProfile,
} from "../app/slices/layoutComponentSlice";
import DateService from "../services/messenger/DateService";
import { getMessageListAsync } from "../app/thunks/MessageThunks";
import { getUserAsync } from "../app/thunks/usersThucks";
import { postJoinToChatAsync } from "../app/thunks/chatsThuck";

export const Chat = () => {
  const authorizationState = useAppSelector(selectAuthorization);
  const currentChatState = useAppSelector(selectCurrentChat);
  const dispatch = useAppDispatch();

  const [showBurgerMenu, setShowBurgerMenu] = useState<boolean>(false);

  const openChatInfo = async () => {
    if (currentChatState.data?.type === ChatTypeEnum.dialog) {
      const user = currentChatState.data.members?.find(
        (m) => m.id !== authorizationState.data?.id
      );

      if (user?.id) {
        await dispatch(getUserAsync(user?.id));
        dispatch(setShowProfile(true));
        dispatch(setShowBlackBackground(true));
      }

      return;
    }

    dispatch(setShowChatInfo(true));
    dispatch(setShowBlackBackground(true));
  };

  const onClickJoinChat = () => {
    if (currentChatState.data) {
      dispatch(postJoinToChatAsync(currentChatState.data?.id));
    }
  };

  useEffect(() => {
    dispatch(setNullMessageData());

    if (currentChatState.data !== null) {
      dispatch(
        getMessageListAsync({
          chatId: currentChatState.data.id,
          limit: 30,
          fromMessageTime: null,
        })
      );
    }
  }, [currentChatState.data]);

  return (
    <div className="chat">
      {showBurgerMenu && (
        <ChatBurgerMenu
          setShowBurgerMenu={setShowBurgerMenu}
          isDialog={currentChatState.data?.type === ChatTypeEnum.dialog}
        />
      )}
      {currentChatState.data ? (
        <>
          <div className="chat__header">
            <img
              className="chat__header__avatar"
              src={
                currentChatState.data?.avatarLink ||
                "https://i.pinimg.com/564x/55/76/29/5576291b493260f343e96dabf11f41d4.jpg"
              }
              alt=""
              onClick={openChatInfo}
            />
            <div className="chat__header__chat-data">
              <p
                className="chat__header__chat-data__title"
                onClick={openChatInfo}
              >
                {currentChatState.data?.title ??
                  currentChatState.data?.members?.find(
                    (m) => m.id !== authorizationState.data?.id
                  )?.displayName}
              </p>
              <p className="chat__header__chat-data__count-members">
                Count members: {currentChatState.data?.membersCount}
              </p>
            </div>
            <div
              className="chat__header__button-menu"
              onMouseEnter={() => setShowBurgerMenu(true)}
            >
              <div className="chat__header__button-menu__dot1" />
              <div className="chat__header__button-menu__dot2" />
              <div className="chat__header__button-menu__dot3" />
            </div>
          </div>
          <div className="chat__messages">
            {currentChatState.data?.banDateOfExpire ? (
              <p className="chat__messages__banned">You are banned</p>
            ) : (
              <>
                {currentChatState.messages.map((m) => (
                  <Message
                    key={m.id}
                    isMyMessage={m.ownerId === authorizationState.data?.id}
                    avatarLink={
                      m.ownerAvatarLink ??
                      "https://i.pinimg.com/564x/55/76/29/5576291b493260f343e96dabf11f41d4.jpg"
                    }
                    text={m.text}
                    time={DateService.getTime(m.dateOfCreate)}
                    isEdit={m.isEdit}
                    OwnerId={m.ownerId ?? ""}
                    displayName={m.ownerDisplayName ?? ""}
                  />
                ))}
              </>
            )}
          </div>
          <div className="chat__footer">
            {currentChatState.data?.isMember ? (
              <>
                <TextArea
                  className="chat__footer__textarea"
                  placeholder="Message"
                  autoSize={{ maxRows: 3 }}
                />
                <SendMessageSVG
                  className="chat__footer__send-message-svg"
                  fill="white"
                />
              </>
            ) : (
              <button
                className="chat__footer__join-button"
                onClick={onClickJoinChat}
              >
                Join
              </button>
            )}
          </div>
        </>
      ) : (
        <p className="chat__select-сhat">Select a сhat</p>
      )}
    </div>
  );
};
