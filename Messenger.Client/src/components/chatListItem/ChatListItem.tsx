import React from "react";
import { observer } from "mobx-react-lite";
import styles from "./ChatListItem.module.scss";
import { ChatType } from "../../models/enum/ChatType";
import { authorizationState } from "../../state/AuthorizationState";
import { IChatListWithMessagesDataItem } from "../../state/types/ChatListWithMessagesStateTypes";
import { currentChatState } from "../../state/CurrentChatState";
import { chatListWithMessagesState } from "../../state/ChatListWithMessagesState";
import nonAvatar from "../../assets/images/non_avatar.jpg";
import { replyState } from "../../state/ReplyState";
import { useNavigate } from "react-router-dom";
import { currentProfileState } from "../../state/CurrentProfileState";
import DateService from "../../services/messenger/DateService";
import { editMessageState } from "../../state/EditMessageState";
import { motion } from "framer-motion";
import { ReactComponent as ConversationLogoSvg } from "../../assets/svg/conversation_logo.svg";
import { ReactComponent as ChannelLogoSvg } from "../../assets/svg/channel_logo.svg";

const ChatListItem = observer((props: IChatListWithMessagesDataItem) => {

  const avatarLink = props.chat.type !== ChatType.Dialog ?
    props.chat.avatarLink || nonAvatar :
    props.chat.members.find(x => x.id !== authorizationState.data?.id)?.avatarLink || nonAvatar;

  const isInChatOnlyRealTimeMessages = props.messages.length !== 0 &&
    props.messages.filter(x => x.isMessageRealtime).length === props.messages.length;
  const firstRealtimeMessage = props.messages.find(x => x.isMessageRealtime);

  const lastMessageTextForChannel = props.chat.lastMessageText ?? "Chat is empty";
  const lastMessageTextForConversationAndDialog = props.chat.lastMessageId ?
    `${props.chat.lastMessageAuthorDisplayName ?? ""}: ${props.chat.lastMessageText ?? ""}`
    : "Chat is empty";

  const navigate = useNavigate();

  const onClickChatItem = async () => {
    if (currentChatState.chat?.id !== props.chat.id) {
      replyState.setReplyNull();
      editMessageState.setEditMessageNull();
    }

    try {
      if (props.messages.length === 0) {
        await chatListWithMessagesState.getMessageListAsync(props.chat.id, null);
      } else if (isInChatOnlyRealTimeMessages) {
        if (!firstRealtimeMessage) return;

        await chatListWithMessagesState.getMessageListAsync(props.chat.id, firstRealtimeMessage?.dateOfCreate);
      }
    } catch (error: any) {
      alert(error.response.data.message);
    }

    currentChatState.setChatAndMessages(props.chat, props.messages);

    if (props.chat.type === ChatType.Dialog) {
      const interlocutorId = currentChatState.chat?.members.find(m => m.id !== authorizationState.data?.id)?.id;

      try {
        if (interlocutorId) {
          await currentProfileState.getUserAsync(interlocutorId);
        }
      } catch (error: any) {
        alert(error.response.data.message);
      }

      return navigate("/", { replace: true });
    }

    return navigate("/chatInfo", { replace: true });
  };

  return (
    <motion.div
      initial={{ opacity: 0.7, y: -5 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: .2 }}
      className={styles.chatListItem} onClick={onClickChatItem}>
      <img
        className={styles.avatar}
        src={avatarLink}
        alt=""
      />
      <div className={styles.container}>
        <p className={styles.chatName}>
          {
            props.chat.type === ChatType.Conversation ? <ConversationLogoSvg className={styles.conversationLogoSvg} width={15} /> :
              props.chat.type === ChatType.Channel ? <ChannelLogoSvg className={styles.conversationLogoSvg} width={14} /> : ""
          }
          <p className={styles.chatNameValue}>
            {
              props.chat.type === ChatType.Dialog
                ? props.chat.members.find(
                  (m) => m.id !== authorizationState.data?.id
                )?.displayName
                : props.chat.title
            }
          </p>
        </p>
        <p className={styles.lastMessage}>
          {
            props.chat.type === ChatType.Channel ?
              lastMessageTextForChannel : lastMessageTextForConversationAndDialog
          }
        </p>
        <p className={styles.date}>
          {
            props.chat.lastMessageDateOfCreate && DateService.getTime(props.chat.lastMessageDateOfCreate)
          }
        </p>
      </div>
    </motion.div>
  );
});

export default ChatListItem;
