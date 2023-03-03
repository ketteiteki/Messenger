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
import TextService from "../../services/messenger/TextService";

const ChatListItem = observer((props: IChatListWithMessagesDataItem) => {

  const navigate = useNavigate();

  const onClickChatItem = async () => {
    if (currentChatState.chat?.id !== props.chat.id) {
      replyState.setReplyNull();
      editMessageState.setEditMessageNull();
    }
    
    if (props.messages.length === 0) {
      await chatListWithMessagesState.getMessageListAsync(props.chat.id, null);
    }

    currentChatState.setChatAndMessages(props.chat, props.messages);

    if (props.chat.type === ChatType.Dialog) {
      const interlocutorId = currentChatState.chat?.members.find(m => m.id !== authorizationState.data?.id)?.id;
      interlocutorId && await currentProfileState.getUserAsync(interlocutorId);
      return navigate("/", {replace: true});
    }

    return navigate("/chatInfo", {replace: true});
  };

  return (
    <div className={styles.chatListItem} onClick={onClickChatItem}>
      <img
        className={styles.avatar}
        src={props.chat.avatarLink ?? nonAvatar}
        alt=""
      />
      <div className={styles.container}>
        <p className={styles.displayName}>
          {props.chat.type === ChatType.Dialog
            ? props.chat.members.find(
                (m) => m.id !== authorizationState.data?.id
              )?.displayName
            : props.chat.title}
        </p>
        <p className={styles.lastMessage}>{TextService.trimTextWithThirdDot(props.chat.lastMessageText ?? "", 20)}</p>
        <p className={styles.date}>{props.chat.lastMessageDateOfCreate && DateService.getTime(props.chat.lastMessageDateOfCreate)}</p>
      </div>
    </div>
  );
});

export default ChatListItem;
