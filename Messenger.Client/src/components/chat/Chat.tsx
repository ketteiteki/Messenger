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

  const messageListRef = useRef<HTMLDivElement>(null);
  const messageListLastElement = useRef<HTMLDivElement>(null);

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

  const editMessageStateData = editMessageState.data;
  const currentChatStateMessagesLength = currentChatState.messages.length;
  const currentChatStateChat = currentChatState.chat;

  const sendMessageHandler = async () => {
    await sendMessage();
  };

  const sendMessageEnterHandler = async (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
    if (!currentChatState.chat) return;
    if (e.key === "Enter") {
      e.preventDefault();

      await sendMessage();
    }
  };

  const sendMessage = async () => {
    if (
      !currentChatState.chat ||
      !authorizationOwnerDisplayName ||
      !currentChatId ||
      !authorizationId ||
      !inputMessage.trim()
    )
      return;

    const messageEntity = new MessageEntity(
      inputMessage,
      authorizationId,
      authorizationOwnerDisplayName,
      authorizationAvatarLink,
      replyState.data?.messageId ?? null,
      replyState.data?.text ?? null,
      replyState.data?.displayName ?? null,
      currentChatId,
      attachmentList.map(_ => new AttachmentEntity(Math.random().toString(), 1, nonAvatar)),
      true
    );

    chatListWithMessagesState.addMessageInData(messageEntity);
    chatListWithMessagesState.setLastMessage(messageEntity);
    chatListWithMessagesState.pushChatOnTop(messageEntity.chatId);

    await chatListWithMessagesState
      .postCreateMessageAsync(messageEntity, attachmentList)
      .catch((error: any) => { if (error.response.status !== 401) alert(error.response.data.message); });

    setAttachmentList([]);
    messageListScrollBottomHandler();
    setInputMessage("");
    editMessageState.setEditMessageNull();
    replyState.setReplyNull();
  };

  const editMessageHandler = async () => {
    if (!currentChatState.chat || !editMessageState.data || !inputMessage.trim())
      return;

    await chatListWithMessagesState.putUpdateMessageAsync(
      currentChatState.chat.id,
      editMessageState.data.messageId,
      inputMessage
    ).catch((error: any) => { if (error.response.status !== 401) alert(error.response.data.message); });

    setInputMessage("");
    editMessageState.setEditMessageNull();
  };

  const editMessageEnterHandler = async (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === "Enter") {
      e.preventDefault();

      if (!currentChatState.chat || !editMessageState.data || !inputMessage.trim())
        return;

      await chatListWithMessagesState.putUpdateMessageAsync(
        currentChatState.chat.id,
        editMessageState.data.messageId,
        inputMessage
      ).catch((error: any) => { if (error.response.status !== 401) alert(error.response.data.message); });

      setInputMessage("");
      editMessageState.setEditMessageNull();
    }
  };

  const onClickGatewayHandler = async () => {
    if (!editMessageState.data) {
      await sendMessageHandler();
    }

    if (editMessageState.data) {
      await editMessageHandler();
    }
  };

  const onEnterGatewayHandler = async (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
    if (!editMessageState.data) {
      await sendMessageEnterHandler(e);
    }

    if (editMessageState.data) {
      await editMessageEnterHandler(e);
    }
  };

  const onClickCloseEditMessageHandler = () => {
    editMessageState.setEditMessageNull();
    setInputMessage("");
  };

  const onChangeAttachmentsHandler = (event: React.FormEvent<HTMLInputElement>) => {
    const files = event.currentTarget.files;

    if (!files || files.length > 4) return;

    let fileArray = Array.from(files);

    setAttachmentList(fileArray);

    const arrayUrlBlob: Array<string | ArrayBuffer | null> = [];

    fileArray.forEach(x => arrayUrlBlob.push(URL.createObjectURL(x)));

    setAttachmentListUrlBlob(arrayUrlBlob);
  }

  const messageListScrollBottomHandler = () => {
    if (!messageListLastElement.current) return;

    messageListLastElement.current.scrollIntoView();
  };

  const onScrollGetMessages = useDebouncedCallback(async () => {
    const messagesOfCurrentChat = chatListWithMessagesState.data.find((x) => x.chat.id === currentChatId)?.messages;

    if (!messagesOfCurrentChat) return;

    const firstMessageArray = messagesOfCurrentChat[messagesOfCurrentChat.length - 1];

    if (!currentChatId || !firstMessageArray || !messageListRef.current)
      return;

    const { scrollTop, scrollHeight, clientHeight } = messageListRef.current;

    if (-scrollTop + clientHeight >= scrollHeight - 10) {
      await chatListWithMessagesState
        .getMessageListAsync(currentChatId, firstMessageArray.dateOfCreate)
        .catch((error: any) => { if (error.response.status !== 401) alert(error.response.data.message); });
    }
  }, 800);

  const onClickJoinChatHandler = async () => {

    if (!currentChatId || !currentChatState.chat) return;

    await currentChatState
      .postJoinToChatAsync(currentChatId)
      .catch((error: any) => { if (error.response.status !== 401) alert(error.response.data.message); });

    chatListWithMessagesState.setSearchInput("");
    chatListWithMessagesState.addChatInData(currentChatState.chat, currentChatState.messages)
    chatListWithMessagesState.resetDataForSearchChats();
    currentChatState.setChatAndMessages(currentChatState.chat, currentChatState.messages);
  }

  const onClickCloseAttachmentPanelHandler = () => {
    setAttachmentList([])
    setAttachmentListUrlBlob([]);
  };

  useEffect(() => {
    if (!editMessageState.data) return;

    setInputMessage(editMessageState.data.text);
  }, [editMessageStateData]);

  useEffect(() => {
    const messageListElement = document.getElementById("messageList");

    if (!messageListElement) return;

    if (messageListElement.clientHeight + 160 >= messageListElement.scrollHeight - messageListElement.scrollTop) {
      messageListScrollBottomHandler();
    }

  }, [currentChatStateMessagesLength]);

  useEffect(() => {
    setAttachmentList([]);
    setAttachmentListUrlBlob([]);
    setInputMessage("");
    messageListScrollBottomHandler();
  }, [currentChatStateChat]);

  return (
    <div className={styles.chat}>
      {
        (banDateOfExpire === null || currentChatState.chat) && (
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
        )
      }
      {
        !currentChatState.chat ? (
          <div className={styles.chooseChat}>
            <motion.p
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: .1 }}
              className={styles.chooseChatPage}>
              Choose a chat
            </motion.p>
          </div>
        ) : banDateOfExpire ? (
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
              ref={messageListRef}
              id="messageList"
              onScroll={onScrollGetMessages}
            >
              <div className={styles.messageListLastElement} ref={messageListLastElement} />
              {currentChatState.messages.map((i) => (
                <Message {...i} key={i.id} />
              ))}
            </div>
            {
              ((isCurrentChatChannel && isCurrentChatMine) || !isCurrentChatChannel) && <div className={styles.footer}>
                {
                  replyState.data &&
                  !banDateOfExpire &&
                  !muteDateOfExpire &&
                  isMember && (
                    <div className={styles.reply}>
                      <ReplySvg className={styles.replySvg} width={20} />
                      <div className={styles.replyContainer}>
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
                  )
                }
                {
                  editMessageState.data &&
                  !banDateOfExpire &&
                  !muteDateOfExpire &&
                  isMember && (
                    <div className={styles.editMessage}>
                      <EditSvg className={styles.editMessageSvg} width={20} />
                      <div className={styles.editMessageContainer}>
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
                  )
                }
                {
                  attachmentList[0] &&
                  !banDateOfExpire &&
                  !muteDateOfExpire &&
                  isMember && (
                    <div className={styles.attachmentPanel}>
                      <div className={styles.attachmentPanelContainer}>
                        {attachmentListUrlBlob.map(x => <img className={styles.attachmentPanelItem} src={x?.toString()} alt={"attachment"} />)}
                      </div>
                      <button
                        className={styles.closeAttachmentButton}
                        onClick={onClickCloseAttachmentPanelHandler}
                      >
                        <CrossSvg width={20} />
                      </button>
                    </div>
                  )
                }
                {
                  !banDateOfExpire &&
                  !muteDateOfExpire &&
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
                  )
                }
                {
                  !isMember && (
                    <button className={styles.JoinChatButton} onClick={onClickJoinChatHandler}>Join</button>
                  )
                }
                {
                  muteDateOfExpire && !banDateOfExpire && (
                    <div className={styles.muted}>
                      <p className={styles.mutedPage}>
                        You are muted. Mute expires:{" "}
                        {DateService.getDateAndTime(muteDateOfExpire || "")}
                      </p>
                    </div>
                  )
                }
              </div>
            }
          </>
        )
      }
    </div>
  );
});

export default Chat;
