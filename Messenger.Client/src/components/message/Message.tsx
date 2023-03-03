import React, { useState } from "react";
import { observer } from "mobx-react-lite";
import styles from "./Message.module.scss";
import MessageBurgerMenu from "../messageBurgerMenu/MessageBurgerMenu";
import IMessageDto from "../../models/interfaces/IMessageDto";
import { authorizationState } from "../../state/AuthorizationState";
import nonAvatar from "../../assets/images/non_avatar.jpg";
import DateService from "../../services/messenger/DateService";
import TextService from "../../services/messenger/TextService";
import { useNavigate } from "react-router-dom";
import {currentProfileState} from "../../state/CurrentProfileState"

const Message = observer((props: IMessageDto) => {
  const [xMousePosition, setXMousePosition] = useState<number>(0);
  const [yMousePosition, setYMousePosition] = useState<number>(0);

  const [showMenu, setShowMenu] = useState<boolean>(false);

  const navigate = useNavigate();

  const isMyMessage = props.ownerId === authorizationState.data?.id;

  const showMenuHandler = (event: React.MouseEvent<HTMLDivElement>) => {
    event.preventDefault();
    setShowMenu(true);
  };

  const showProfileByMessage = async () => {
    if (props.ownerId === null) return;

    await currentProfileState.getUserAsync(props.ownerId);

    return navigate("/", {replace: true});
  };

  const MouseMoveHandler = (event: React.MouseEvent<HTMLDivElement>) => {
    var targetCoords = event.currentTarget.getBoundingClientRect();
    var localX = event.clientX - targetCoords.left;
    var localY = event.clientY - targetCoords.top;

    setXMousePosition(localX);
    setYMousePosition(localY);
  };

  return (
    <>
      {isMyMessage ? (
        <>
          <div
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
                isMyMessage={isMyMessage}
                setShowMenu={setShowMenu}
              />
            )}
            <img
              className={styles.myAvatar}
              src={
                props.ownerAvatarLink !== null
                  ? `${props.ownerAvatarLink}`
                  : nonAvatar
              }
              alt="avatar"
            />
            <div className={styles.myMessageData}>
              {props.replyToMessageId !== null && (
                <div className={styles.myMessageReply}>
                  <p className={styles.myMessageReplyDisplayName}>
                    {props.replyToMessageAuthorDisplayName}
                  </p>
                  <p className={styles.myMessageReplyText}>
                    {TextService.trimTextWithThirdDot(
                      props.replyToMessageText || "",
                      35
                    )}
                  </p>
                </div>
              )}
              <p className={styles.myText}>{props.text}</p>
              <p className={styles.myMetaData}>
                {props.isEdit ? "edit" : ""}{" "}
                {DateService.getTime(props.dateOfCreate)}
              </p>
            </div>
          </div>
        </>
      ) : (
        <div
          className={styles.message}
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
              isMyMessage={isMyMessage}
              setShowMenu={setShowMenu}
            />
          )}
          <img
            className={styles.avatar}
            src={
              props.ownerAvatarLink !== null
                ? `${props.ownerAvatarLink}`
                : nonAvatar
            }
            alt="avatar"
          />
          <div className={styles.messageData}>
            <p className={styles.nickname} onClick={showProfileByMessage}>{props.ownerDisplayName}</p>
            {props.replyToMessageId && (
              <div className={styles.messageReply}>
                <p className={styles.messageReplyDisplayName}>
                  {props.replyToMessageAuthorDisplayName}
                </p>
                <p className={styles.messageReplyText}>
                  {TextService.trimTextWithThirdDot(
                    props.replyToMessageText || "",
                    35
                  )}
                </p>
              </div>
            )}
            <p className={styles.text}>{props.text}</p>
            <p className={styles.metaData}>
              {props.isEdit ? "edit" : ""}{" "}
              {DateService.getTime(props.dateOfCreate)}
            </p>
          </div>
        </div>
      )}
    </>
  );
});

export default Message;
