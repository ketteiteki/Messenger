import React from "react";
import { useAppDispatch } from "../app/hooks";
import { setShowBlackBackground, setShowProfile } from "../app/slices/layoutComponentSlice";
import { getUserAsync } from "../app/thunks/usersThucks";

interface IMessageProps {
  isMyMessage: boolean;
  isEdit: boolean;
  avatarLink: string | null;
  text: string;
  time: string;
  displayName: string;
  OwnerId: string
}

export const Message = ({ isMyMessage, avatarLink, text, time, isEdit, displayName, OwnerId }: IMessageProps) => {

  const dispatch = useAppDispatch();

  const onClickOpenProfile = async () => {
    await dispatch(getUserAsync(OwnerId));
    dispatch(setShowBlackBackground(true));
    dispatch(setShowProfile(true));
  }

  return (
    <>
      {isMyMessage ? (
        <div className="chat__messages__my-message">
          <img
            className="chat__messages__my-message__avatar"
            src={`${avatarLink}`}
            alt="avatar"
          />
          <div className="chat__messages__my-message__message-data">
            <p className="chat__messages__my-message__message-data__text">
						{text}
            </p>
            <p className="chat__messages__my-message__message-data__meta-data">
              {isEdit ? "edited" : ""} {time}
            </p>
          </div>
        </div>
      ) : (
        <div className="chat__messages__message">
          <img
            className="chat__messages__message__avatar"
            onClick={onClickOpenProfile}
            src={`${avatarLink}`}
            alt="avatar"
          />
          <div className="chat__messages__message__message-data">
            <p className="chat__messages__message__message-data__nickname">
              {displayName}
            </p>
            <p className="chat__messages__message__message-data__text">
              {text}
            </p>
            <p className="chat__messages__message__message-data__meta-data">
              {isEdit ? "edit" : ""} {time}
            </p>
          </div>
        </div>
      )}
    </>
  );
};
