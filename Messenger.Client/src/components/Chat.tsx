import React, { useEffect, useState } from "react";
import TextArea from "rc-textarea";
import { ReactComponent as SendMessageSVG } from "../assets/svg/send-message.svg";
import { Message } from "./Message";
import { useAppDispatch, useAppSelector } from "../app/hooks";
import { v4 as uuidv4 } from "uuid";
import {
  selectCurrentChat,
  currentChatSliceActions,
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
import { getMessageListAsync } from "../app/thunks/messageThunks";
import { getUserAsync } from "../app/thunks/usersThucks";
import { postJoinToChatAsync } from "../app/thunks/chatsThuck";
import MessageEntity from "../models/MessageEntity";
import MessagesAPI from "../services/api/MessagesAPI";
import {
  chatListSliceActions,
  selectChatList,
} from "../app/slices/chatListSlice";

export const Chat = () => {
  const authorizationState = useAppSelector(selectAuthorization);
  const currentChatState = useAppSelector(selectCurrentChat);
  const chatListState = useAppSelector(selectChatList);
  const dispatch = useAppDispatch();

  const [showBurgerMenu, setShowBurgerMenu] = useState<boolean>(false);
  const [textareaForSendMessageText, setTextareaForSendMessageText] =
    useState<string>("");
  const [loadingGetMessage, setLoadingGetMessage] = useState<boolean>(false);

  const refChatMessages = React.createRef<HTMLDivElement>();

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

  const onClickJoinChat = async () => {
    setLoadingGetMessage(true);
    if (currentChatState.data) {
      await dispatch(postJoinToChatAsync(currentChatState.data?.id));
    }
    setLoadingGetMessage(false)
  };

  const onScrollGetMessages = () => {
    if (refChatMessages.current?.scrollTop === 0 && currentChatState.data) {
      dispatch(
        getMessageListAsync({
          chatId: currentChatState.data.id,
          limit: 15,
          fromMessageTime: currentChatState.messages[0].dateOfCreate,
        })
      );
    }
  }

  const SendMessage = async () => {
    if (authorizationState.data && currentChatState.data) {
      const messageId = uuidv4();

      const message = new MessageEntity(
        messageId,
        textareaForSendMessageText,
        false,
        authorizationState.data.id || "",
        authorizationState.data.displayName || "",
        authorizationState.data.avatarLink || null,
        null,
        null,
        null,
        [],
        currentChatState.data.id,
        `${new Date()}`
      );

      await dispatch(currentChatSliceActions.addMessageCurrentChat(message));
      await dispatch(chatListSliceActions.addMessageInChatOfChatList(message));

      const createMessageResult = await MessagesAPI.postCreateMessageAsync(
        textareaForSendMessageText,
        currentChatState.data.id,
        null,
        []
      );

      await dispatch(
        currentChatSliceActions.updateMessageIdAfterCreateMessage({
          lastMessageId: messageId,
          message: createMessageResult.data,
        })
      );
      await dispatch(
        chatListSliceActions.updateMessageIdAfterCreateMessage({
          lastMessageId: messageId,
          message: createMessageResult.data,
        })
      );
      await dispatch(chatListSliceActions.setLastMessage(createMessageResult.data));

      setTextareaForSendMessageText("");
      refChatMessagesScrollBottom();
    }
  };

  const onClickSendMessage = () => SendMessage();

  const onEnterSendMessage = (e: React.KeyboardEvent) => {
    e.preventDefault();
    if (e.key === "Enter") {
      return SendMessage();
    }
  };

  const refChatMessagesScrollBottom = () => {
    if (refChatMessages.current) {
      refChatMessages.current.scrollTop = refChatMessages.current.scrollHeight;
    }
  };

  useEffect(() => {
    refChatMessagesScrollBottom();
  }, [currentChatState.data]);

  useEffect(() => {
    const fun = async () => {
      if (
        currentChatState.data !== null &&
        chatListState.data.find((c) => c.chat.id === currentChatState.data?.id)
          ?.messages.length === 0
      ) {
        await dispatch(
          getMessageListAsync({
            chatId: currentChatState.data.id,
            limit: 15,
            fromMessageTime: null,
          })
        );
      }
    };

    fun()
  }, [currentChatState.data?.id]);

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
          <div className="chat__messages" ref={refChatMessages} onScroll={onScrollGetMessages}>
            {loadingGetMessage && <div className="chat__messages__loading"/>}
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
                  value={textareaForSendMessageText}
                  onChange={(e) =>
                    setTextareaForSendMessageText(e.currentTarget.value)
                  }
                  onPressEnter={onEnterSendMessage}
                />
                <SendMessageSVG
                  className="chat__footer__send-message-svg"
                  fill="white"
                  onClick={onClickSendMessage}
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
