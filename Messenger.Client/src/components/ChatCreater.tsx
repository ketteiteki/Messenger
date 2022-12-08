import React, { useState } from "react";
import { useAppDispatch } from "../app/hooks";
import { setShowBlackBackground, setShowChatCreater } from "../app/slices/layoutComponentSlice";
import { postCreateChatAsync } from "../app/thunks/chatsThuck";
import { CreateChatEnum } from "../models/enums/CreateChatEnum";


export const ChatCreater = () => {
  const dispatch = useAppDispatch();

  const [inputChatAvatarFile, setInputChatAvatarFile] = useState<File>();
  const [inputChatTitle, setInputChatTitle] = useState<string>("");
  const [inputChatName, setInputChatName] = useState<string>("");
  const [selectChatType, setSelectChatType] = useState<number>(CreateChatEnum.conversation);

  const onChangeChatAvatarFileInput = (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    if (e.currentTarget.files) {
      setInputChatAvatarFile(e.currentTarget.files[0]);
    }
  };

  const onChangeChatTitleInput = (e: React.ChangeEvent<HTMLInputElement>) => {
    setInputChatTitle(e.currentTarget.value);
  };

  const onChangeChatNameInput = (e: React.ChangeEvent<HTMLInputElement>) => {
    setInputChatName(e.currentTarget.value);
  };

  const onClickCreateChatButton = async (e: React.MouseEvent<HTMLButtonElement>) => {
    await dispatch(
      postCreateChatAsync({
        title: inputChatTitle,
        name: inputChatName,
        type: selectChatType,
        avatarFile: inputChatAvatarFile ?? null,
      })
    );
    
    dispatch(setShowChatCreater(false));
    dispatch(setShowBlackBackground(false));
  };

  return (
    <div className="chat-creater">
      <label className="chat-creater__file-input-label" htmlFor="file">
        {inputChatAvatarFile ? "File uploaded" : " Choose a file"}
      </label>
      <input
        type="file"
        name="file"
        id="file"
        onChange={onChangeChatAvatarFileInput}
        accept=".png, .jpg, .jpeg"
        className="chat-creater__file-input"
      />
      <input
        type="text"
        className="chat-creater__title-input"
        value={inputChatTitle}
        onChange={onChangeChatTitleInput}
        placeholder="Title"
      />
      <input
        type="text"
        className="chat-creater__name-input"
        value={inputChatName}
        onChange={onChangeChatNameInput}
        placeholder="Name"
      />
      <select
        className="chat-creater__chat-type-select"
        onChange={(e) => setSelectChatType(Number.parseInt(e.currentTarget.value))}
      >
        <option disabled defaultValue={"Choose a type of chat"}>
          Choose a type of chat
        </option>
        <option value={CreateChatEnum.conversation}>Conversation</option>
        <option value={CreateChatEnum.channel}>Channel</option>
      </select>
      <button
        className="chat-creater__create-chat-button"
        onClick={onClickCreateChatButton}
      >
        Create
      </button>
    </div>
  );
};
