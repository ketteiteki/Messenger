import React, { useState } from "react";
import { observer } from "mobx-react-lite";
import styles from "./CreateChat.module.scss";
import { ChatType } from "../../models/enum/ChatType";
import { chatListWithMessagesState } from "../../state/ChatListWithMessagesState";

const CreateChat = observer(() => {
  const [inputName, setInputName] = useState<string>("");
  const [inputTitle, setInputTitle] = useState<string>("");

  const [selectChatTypeValue, setSelectChatTypeValue] = useState<number>(1);

  const [file, setFile] = useState<File>();

  const setFileHandler = (event: React.FormEvent<HTMLInputElement>) => {
    const files = event.currentTarget.files;

    if (files && files.length > 0) {
      setFile(files[0]);
    }
  };

  const createChatHandler = async () => {
    await chatListWithMessagesState.postCreateChatAsync(
      inputName,
      inputTitle,
      selectChatTypeValue,
      file ?? null
    );
  };

  return (
    <div className={styles.createChat}>
      <div className={styles.createChatContainer}>
        <input type="file" onChange={setFileHandler} />
        <input
          className={styles.inputTitle}
          type="text"
          placeholder="Title"
          value={inputName}
          onChange={(e) => setInputName(e.currentTarget.value)}
        />
        <input
          className={styles.inputName}
          type="text"
          placeholder="Name"
          value={inputTitle}
          onChange={(e) => setInputTitle(e.currentTarget.value)}
        />
        <select
          className={styles.chooseChatTypeSelect}
          onChange={(e) =>
            setSelectChatTypeValue(Number(e.currentTarget.value))
          }
        >
          <option value={ChatType.Channel}>Channel</option>
          <option value={ChatType.Conversation}>Conversation</option>
        </select>
        <button className={styles.CreateChatButton} onClick={createChatHandler}>
          Create Chat
        </button>
      </div>
    </div>
  );
});

export default CreateChat;
