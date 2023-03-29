import React, { useState, useEffect, useRef } from "react";
import { observer } from "mobx-react-lite";
import styles from "./Chat.module.scss";
import TextArea from "rc-textarea";
import { ReactComponent as SendMessageSvg } from "../../assets/svg/send_message.svg";
import { ReactComponent as AttachmentSvg } from "../../assets/svg/attachment.svg";
import { ReactComponent as ReplySvg } from "../../assets/svg/reply_black.svg";
import { ReactComponent as CrossSvg } from "../../assets/svg/cross.svg";
import { ReactComponent as EditSvg } from "../../assets/svg/edit_black.svg";
import Message from "../message/Message";
import { currentChatState } from "../../state/CurrentChatState";
import DateService from "../../services/messenger/DateService";
import { ChatType } from "../../models/enum/ChatType";
import { authorizationState } from "../../state/AuthorizationState";
import nonAvatar from "../../assets/images/non_avatar.jpg";
import { replyState } from "../../state/ReplyState";
import { chatListWithMessagesState } from "../../state/ChatListWithMessagesState";
import { editMessageState } from "../../state/EditMessageState";
import MessageEntity from "../../models/entities/MessageEntity";
import { useDebouncedCallback } from "use-debounce";
import AttachmentEntity from "../../models/entities/AttachmentEntity";
import { motion } from "framer-motion";

const Chat = observer(() => {
  const [inputMessage, setInputMessage] = useState<string>("");
  const [attachmentList, setAttachmentList] = useState<File[]>([]);
  const [attachmentListUrlBlob, setAttachmentListUrlBlob] = useState<Array<string | ArrayBuffer | null>>([]);

  const refMessageList = document.getElementById("messageList");

  const banDateOfExpire = currentChatState.chat?.banDateOfExpire;
  const muteDateOfExpire = currentChatState.chat?.banDateOfExpire;
  const isMember = currentChatState.chat?.isMember;
  const interlocutorDisplayName = currentChatState.chat?.members.find(
    (m) => m.id !== authorizationState.data?.id
  )?.displayName;
  const interlocutorNickname = currentChatState.chat?.members.find(
    (m) => m.id !== authorizationState.data?.id
  )?.nickname;
  const authorizationId = authorizationState.data?.id;
  const authorizationOwnerDisplayName = authorizationState.data?.displayName;
  const authorizationAvatarLink = authorizationState.data?.avatarLink ?? null;
  const currentChatId = currentChatState.chat?.id;
  const isCurrentChatChannel = currentChatState.chat?.type === ChatType.Channel;
  const isCurrentChatMine = currentChatState.chat?.isOwner;
  const currentChatAvatar = currentChatState.chat?.type !== ChatType.Dialog ?
    currentChatState.chat?.avatarLink ?? nonAvatar
    : currentChatState.chat.members.find(x => x.id !== authorizationState.data?.id)?.avatarLink ?? nonAvatar;

  const sendMessageHandler = async () => {
    await sendMessage();
  };

  const sendMessageEnterHandler = async (
    e: React.KeyboardEvent<HTMLTextAreaElement>
  ) => {
    if (currentChatState.chat === null) return;
    if (e.key === "Enter") {
      e.preventDefault();

      await sendMessage();
    }
  };

  const sendMessage = async () => {
    if (
      currentChatState.chat === null ||
      authorizationOwnerDisplayName === undefined ||
      currentChatId === undefined ||
      authorizationId === undefined
    ) return;

    if (inputMessage === "") return;

    const messageEntity = new MessageEntity(
      inputMessage,
      authorizationId,
      authorizationOwnerDisplayName,
      authorizationAvatarLink,
      replyState.data?.messageId ?? null,
      replyState.data?.text ?? null,
      replyState.data?.displayName ?? null,
      currentChatId,
      attachmentList.map(x => new AttachmentEntity(Math.random().toString(), 1, nonAvatar)),
      true
    );

    chatListWithMessagesState.addMessageInData(messageEntity);
    chatListWithMessagesState.setLastMessage(messageEntity);
    chatListWithMessagesState.pushChatOnTop(messageEntity.chatId);

    await chatListWithMessagesState.postCreateMessageAsync(messageEntity, attachmentList);

    setAttachmentList([]);
    messageListScrollBottomHandler();
    setInputMessage("");
    editMessageState.setEditMessageNull();
    replyState.setReplyNull();
  };

  const editMessageHandler = async () => {
    if (currentChatState.chat === null || editMessageState.data === null)
      return;

    await chatListWithMessagesState.putUpdateMessageAsync(
      currentChatState.chat.id,
      editMessageState.data.messageId,
      inputMessage
    );

    setInputMessage("");
    editMessageState.setEditMessageNull();
  };

  const editMessageEnterHandler = async (
    e: React.KeyboardEvent<HTMLTextAreaElement>
  ) => {
    if (currentChatState.chat === null || editMessageState.data === null)
      return;

    if (e.key === "Enter") {
      e.preventDefault();

      await chatListWithMessagesState.putUpdateMessageAsync(
        currentChatState.chat.id,
        editMessageState.data.messageId,
        inputMessage
      );

      setInputMessage("");
      editMessageState.setEditMessageNull();
    }
  };

  const onClickGatewayHandler = () => {
    if (editMessageState.data === null) {
      sendMessageHandler();
    }

    if (editMessageState !== null) {
      editMessageHandler();
    }
  };

  const onEnterGatewayHandler = async (
    e: React.KeyboardEvent<HTMLTextAreaElement>
  ) => {
    if (editMessageState.data === null) {
      sendMessageEnterHandler(e);
    }

    if (editMessageState !== null) {
      editMessageEnterHandler(e);
    }
  };

  const onClickCloseEditMessageHandler = () => {
    editMessageState.setEditMessageNull();
    setInputMessage("");
  };

  const onChangeAttachmentsHandler = (event: React.FormEvent<HTMLInputElement>) => {
    const files = event.currentTarget.files;

    if (!files || files.length > 4) return;

    var fileArray = Array.from(files);

    setAttachmentList(fileArray);

    const arrayUrlBlob: Array<string | ArrayBuffer | null> = [];

    fileArray.forEach(x => arrayUrlBlob.push(URL.createObjectURL(x)));

    setAttachmentListUrlBlob(arrayUrlBlob);
  }

  const messageListScrollBottomHandler = () => {
    if (refMessageList === null) return;

    refMessageList.scrollTop = 1000000;
  };

  const getMessages = useDebouncedCallback(async () => {
    const messageListElement = document.getElementById("messageList");

    const firstMessageInArray = chatListWithMessagesState.data.find(
      (x) => x.chat.id === currentChatId
    )?.messages[0];

    if (
      messageListElement === null ||
      currentChatId === undefined ||
      firstMessageInArray === undefined
    )
      return;

    if (messageListElement?.scrollTop === 0) {
      await chatListWithMessagesState.getMessageListAsync(currentChatId, firstMessageInArray.dateOfCreate);
    }
  }, 800);

  const joinChatHandler = async () => {

    if (currentChatId === undefined || currentChatState.chat === null) return;

    const response = await currentChatState.postJoinToChatAsync(currentChatId);

    chatListWithMessagesState.addChatInData(currentChatState.chat, currentChatState.messages)
    chatListWithMessagesState.resetDataForSearchChats();
    currentChatState.setChatAndMessages(currentChatState.chat, currentChatState.messages);
    chatListWithMessagesState.setSearchInput("");
  }

  const onClickCloseAttachmentPanelHandler = () => {
    setAttachmentList([])
    setAttachmentListUrlBlob([]);
  };

  useEffect(() => {
    if (editMessageState.data === null) return;

    setInputMessage(editMessageState.data.text);
  }, [editMessageState.data]);

  useEffect(() => {
    setAttachmentList([]);
    setAttachmentListUrlBlob([]);
    setInputMessage("");
    messageListScrollBottomHandler();
  }, [currentChatState.chat]);

  return (
    <div className={styles.chat}>
      {(banDateOfExpire === null || currentChatState.chat !== null) && (
        <div className={styles.header}>
          <p className={styles.chatName}>
            {currentChatState.chat?.type !== ChatType.Dialog
              ? currentChatState.chat?.title
              : interlocutorDisplayName}
          </p>
          <p className={styles.additionalData}>
            {currentChatState.chat?.type !== ChatType.Dialog
              ? `Members: ${currentChatState.chat?.membersCount}`
              : interlocutorNickname}
          </p>
          <img
            className={styles.chatAvatar}
            src={currentChatAvatar}
            alt=""
          />
        </div>
      )}
      {currentChatState.chat === null ? (
        <div className={styles.chooseChat}>
          <motion.p
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{ duration: .1 }}
            className={styles.chooseChatPage}>
            Choose a chat
          </motion.p>
        </div>
      ) : banDateOfExpire !== null ? (
        <div className={styles.banned}>
          <p className={styles.bannedPage}>
            You are banned. Ban expires:{" "}
            {DateService.getDateAndTime(banDateOfExpire!)}
          </p>
        </div>
      ) : (
        <>
          <div
            className={styles.messageList}
            id="messageList"
            onScroll={getMessages}
          >
            {currentChatState.messages.map((i) => (
              <Message {...i} key={i.id} />
            ))}
          </div>
          {((isCurrentChatChannel && isCurrentChatMine) || !isCurrentChatChannel) && <div className={styles.footer}>
            {replyState.data !== null &&
              banDateOfExpire === null &&
              muteDateOfExpire === null &&
              isMember && (
                <div className={styles.reply}>
                  <ReplySvg className={styles.replySvg} width={20} />
                  <div className={styles.replyConstainer}>
                    <p className={styles.replyDisplayName}>
                      {replyState.data.displayName}
                    </p>
                    <p className={styles.replyMessageText}>
                      {replyState.data.text}
                    </p>
                  </div>
                  <button
                    className={styles.closeReplyButton}
                    onClick={replyState.setReplyNull}
                  >
                    <CrossSvg width={20} />
                  </button>
                </div>
              )}
            {editMessageState.data !== null &&
              banDateOfExpire === null &&
              muteDateOfExpire === null &&
              isMember && (
                <div className={styles.editMessage}>
                  <EditSvg className={styles.editMessageSvg} width={20} />
                  <div className={styles.editMessageConstainer}>
                    <p className={styles.editMessagePage}>Edit Message</p>
                    <p className={styles.editMessageText}>
                      {editMessageState.data.text}
                    </p>
                  </div>
                  <button
                    className={styles.closeEditMessageButton}
                    onClick={onClickCloseEditMessageHandler}
                  >
                    <CrossSvg width={20} />
                  </button>
                </div>
              )}
            {attachmentList[0] &&
              banDateOfExpire === null &&
              muteDateOfExpire === null &&
              isMember && (
                <div className={styles.attachmentPanel}>
                  <div className={styles.attachmentPanelConstainer}>
                    {attachmentListUrlBlob.map(x => <img className={styles.attachmentPanelItem} src={x?.toString()} />)}
                  </div>
                  <button
                    className={styles.closeAttachmentButton}
                    onClick={onClickCloseAttachmentPanelHandler}
                  >
                    <CrossSvg width={20} />
                  </button>
                </div>
              )}
            {banDateOfExpire === null &&
              muteDateOfExpire === null &&
              isMember && (
                <>
                  <input
                    className={styles.attachmentInput}
                    onChange={onChangeAttachmentsHandler}
                    type="file"
                    id="attachment"
                    name="attachment"
                    multiple
                    accept="image/jpeg"
                  />
                  <label htmlFor="attachment" className={styles.attachmentLabel}>
                    <AttachmentSvg className={styles.attachmentSvg} width={25} />
                  </label>
                  <TextArea
                    id="sendMessageTextArea"
                    className={styles.textarea}
                    autoSize={{ maxRows: 3 }}
                    placeholder="Send Message"
                    value={inputMessage}
                    onChange={(e) => setInputMessage(e.currentTarget.value)}
                    onKeyDown={onEnterGatewayHandler}
                  />
                  <SendMessageSvg
                    className={styles.sendMessageSvg}
                    width={30}
                    onClick={onClickGatewayHandler}
                  />
                </>
              )}
            {isMember === false && (
              <button className={styles.JoinChatButton} onClick={joinChatHandler}>Join</button>
            )}
            {muteDateOfExpire !== null && banDateOfExpire === null && (
              <div className={styles.muted}>
                <p className={styles.mutedPage}>
                  You are muted. Mute expires:{" "}
                  {DateService.getDateAndTime(muteDateOfExpire || "")}
                </p>
              </div>
            )}
          </div>}
        </>
      )}
    </div>
  );
});

export default Chat;
