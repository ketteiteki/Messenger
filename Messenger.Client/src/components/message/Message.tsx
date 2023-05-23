import React, { useState } from "react";
import { observer } from "mobx-react-lite";
import styles from "./Message.module.scss";
import MessageBurgerMenu from "../messageBurgerMenu/MessageBurgerMenu";
import IMessageDto from "../../models/interfaces/IMessageDto";
import { authorizationState } from "../../state/AuthorizationState";
import nonAvatar from "../../assets/images/non_avatar.jpg";
import DateService from "../../services/messenger/DateService";
import { useNavigate } from "react-router-dom";
import { currentProfileState } from "../../state/CurrentProfileState"
import { currentChatState } from "../../state/CurrentChatState";
import { ChatType } from "../../models/enum/ChatType";
import { blackCoverState } from "../../state/BlackCoverState";
import { ReactComponent as ClockSvg } from "../../assets/svg/clock.svg";
import { motion } from "framer-motion";

const Message = observer((props: IMessageDto) => {
  const [xMousePosition, setXMousePosition] = useState<number>(0);
  const [yMousePosition, setYMousePosition] = useState<number>(0);

  const [showMenu, setShowMenu] = useState<boolean>(false);

  const navigate = useNavigate();

  const messageAttachmentListStyle = props.attachments[3] ?
    styles.messageAttachmentListFourAttachment
    : props.attachments[2]
      ? styles.messageAttachmentListThreeAttachment
      : props.attachments[1]
        ? styles.messageAttachmentListTwoAttachment
        : styles.messageAttachmentListOneAttachment;

  const isMessageMine = props.ownerId === authorizationState.data?.id;
  const isCurrentChatChannel = currentChatState.chat?.type === ChatType.Channel;
  const isCurrentChatMine = currentChatState.chat?.isOwner;
  const isMessageLast = currentChatState.messages.findIndex(x => x.id === props.id) === 0;

  const showMenuHandler = (event: React.MouseEvent<HTMLDivElement>) => {
    event.preventDefault();
    setShowMenu(true);
  };

  const showProfileByMessage = async () => {
    if (props.ownerId === null) return;

    if (isMessageMine) {
      currentProfileState.setProfileNull();

      return navigate("/", { replace: true });
    }

    await currentProfileState.getUserAsync(props.ownerId);

    return navigate("/", { replace: true });
  };

  const MouseMoveHandler = (event: React.MouseEvent<HTMLDivElement>) => {
    var targetCoords = event.currentTarget.getBoundingClientRect();
    var localX = event.clientX - targetCoords.left;
    var localY = event.clientY - targetCoords.top;

    setXMousePosition(localX);
    setYMousePosition(localY);
  };

  const onClickOpenFullSizeAvatar = (imageLink: string) => {
    blackCoverState.setImage(imageLink);
  };

  return (
    <>
      {isMessageMine ? (
        <>
          <motion.div
            initial={{ opacity: 0.7 }}
            animate={{ opacity: 1 }}
            transition={{ duration: .1 }}
            className={styles.myMessage}
            onContextMenu={showMenuHandler}
            onMouseMove={MouseMoveHandler}
          >
            {showMenu && (
              <MessageBurgerMenu
                x={xMousePosition}
                y={yMousePosition}
                messageId={props.id}
                chatId={props.chatId}
                displayName={props.ownerDisplayName || ""}
                text={props.text}
                isMyMessage={isMessageMine}
                isMessageLast={isMessageLast}
                setShowMenu={setShowMenu}
              />
            )}
            <img
              className={styles.myAvatar}
              src={
                currentChatState.chat?.type === ChatType.Channel ?
                  currentChatState.chat.avatarLink ?? nonAvatar
                  : props.ownerAvatarLink ?? nonAvatar
              }
              onClick={showProfileByMessage}
              alt="avatar"
            />
            <div className={styles.myMessageData}>
              {props.replyToMessageId !== null && (
                <div className={styles.myMessageReply}>
                  <p className={styles.myMessageReplyDisplayName}>
                    {props.replyToMessageAuthorDisplayName}
                  </p>
                  <p className={styles.myMessageReplyText}>
                    {props.replyToMessageText || ""}
                  </p>
                </div>
              )}
              {props.attachments[0] && <div className={messageAttachmentListStyle}>
                {props.attachments.map(x =>
                  <img className={styles.messageAttachment} onClick={() => onClickOpenFullSizeAvatar(x.link)} src={x.link} alt="" />)}
              </div>}
              <p className={styles.myText}>{props.text}</p>
              <p className={styles.myMetaData}>
                {props.loading && <ClockSvg className={styles.clockSvg} width={10} fill={"#fff"} />}
                {props.isEdit ? "edit" : ""}{" "}
                {DateService.getTime(props.dateOfCreate)}
              </p>
            </div>
          </motion.div>
        </>
      ) : (
        <motion.div
          initial={{ opacity: .5 }} animate={{ opacity: 1 }}
          className={styles.message}
          onContextMenu={showMenuHandler}
          onMouseMove={MouseMoveHandler}
        >
          {(showMenu && isCurrentChatChannel && isCurrentChatMine) ||
            (showMenu && !isCurrentChatChannel) && (
              <MessageBurgerMenu
                x={xMousePosition}
                y={yMousePosition}
                messageId={props.id}
                chatId={props.chatId}
                displayName={props.ownerDisplayName || ""}
                text={props.text}
                isMyMessage={isMessageMine}
                isMessageLast={isMessageLast}
                setShowMenu={setShowMenu}
              />
            )}
          <img
            className={styles.avatar}
            src={
              currentChatState.chat?.type === ChatType.Channel ?
                currentChatState.chat.avatarLink ?? nonAvatar
                : props.ownerAvatarLink ?? nonAvatar
            }
            onClick={showProfileByMessage}
            alt="avatar"
          />
          <div className={styles.messageData}>
            {!props.attachments[0] &&
              <p className={styles.nickname} onClick={showProfileByMessage}>
                {currentChatState.chat?.type !== ChatType.Channel ?
                  props.ownerDisplayName
                  : currentChatState.chat.title}</p>}
            {props.replyToMessageId && (
              <div className={styles.messageReply}>
                <p className={styles.messageReplyDisplayName}>
                  {currentChatState.chat?.type !== ChatType.Channel ?
                    props.replyToMessageAuthorDisplayName
                    : currentChatState.chat.title}
                </p>
                <p className={styles.messageReplyText}>
                  {props.replyToMessageText || ""}
                </p>
              </div>
            )}
            {props.attachments[0] && <div className={messageAttachmentListStyle}>
              {props.attachments.map(x =>
                <img className={styles.messageAttachment} onClick={() => onClickOpenFullSizeAvatar(x.link)} src={x.link} alt="" />)}
            </div>}
            <p className={styles.text}>{props.text}</p>
            <p className={styles.metaData}>
              {props.isEdit ? "edit" : ""}{" "}
              {DateService.getTime(props.dateOfCreate)}
            </p>
          </div>
        </motion.div>
      )}
    </>
  );
});

export default Message;
