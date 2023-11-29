import React, { useState } from "react";
import { observer } from "mobx-react-lite";
import styles from "./MessageBurgerMenu.module.scss";
import { ReactComponent as EditSvg } from "../../assets/svg/edit_black.svg";
import { ReactComponent as TrashBinSvg } from "../../assets/svg/trash_bin_black.svg";
import { ReactComponent as ReplySvg } from "../../assets/svg/reply_black.svg";
import { replyState } from "../../state/ReplyState";
import ReplyEntity from "../../models/entities/ReplyEntity";
import EditMessageEntity from "../../models/entities/EditMessageEntity";
import { editMessageState } from "../../state/EditMessageState";
import { chatListWithMessagesState } from "../../state/ChatListWithMessagesState";

interface IMessageBurgerMenu {
  x: number;
  y: number;
  messageId: string;
  chatId: string;
  displayName: string;
  text: string;
  isMyMessage: boolean;
  isMessageLast: boolean;
  setShowMenu: React.Dispatch<React.SetStateAction<boolean>>;
}

const MessageBurgerMenu = observer(
  ({
    x,
    y,
    isMyMessage,
    messageId,
    chatId,
    displayName,
    text,
    isMessageLast,
    setShowMenu,
  }: IMessageBurgerMenu) => {
    const [xMousePosition] = useState<number>(x - 5);
    const [yMousePosition] = useState<number>(y - 5);

    const textareaSendMessageElement = document.getElementById("sendMessageTextArea");

    const setReplyHandler = () => {
      const replyEntity = new ReplyEntity(messageId, displayName, text);

      editMessageState.setEditMessageNull();

      replyState.setReply(replyEntity);
      textareaSendMessageElement?.focus();
    };

    const setEditMessageHandler = () => {
      const editMessageEntity = new EditMessageEntity(messageId, chatId, displayName, text);

      replyState.setReplyNull();

      editMessageState.setEditMessage(editMessageEntity);
      textareaSendMessageElement?.focus();
    };

    const deleteMessage = async () => {
      await chatListWithMessagesState
        .delDeleteMessageAsync(chatId, messageId)
        .catch((error: any) => { if (error.response.status !== 401) alert(error.response.data.message); });
    }

    return (
      <div
        className={styles.messageBurgerMenu}
        style={{
          left: `${xMousePosition}px`,
          top: isMessageLast && isMyMessage ? `${yMousePosition - 80}px` : `${yMousePosition}px`
        }}
        onMouseLeave={() => setShowMenu(false)}
      >
        <button className={styles.replyMessageButton} onClick={setReplyHandler}>
          <ReplySvg width={20} />
          <p>Reply</p>
        </button>
        {
          isMyMessage && (
            <>
              <button className={styles.updateMessageButton} onClick={setEditMessageHandler}>
                <EditSvg width={20} />
                <p>Edit</p>
              </button>
              <button className={styles.deleteMessageButton} onClick={deleteMessage}>
                <TrashBinSvg width={20} />
                <p>Delete</p>
              </button>
            </>
          )
        }
      </div>
    );
  }
);

export default MessageBurgerMenu;
