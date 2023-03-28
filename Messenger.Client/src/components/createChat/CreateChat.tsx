import React, { useState } from "react";
import { observer } from "mobx-react-lite";
import styles from "./CreateChat.module.scss";
import { ChatType } from "../../models/enum/ChatType";
import { chatListWithMessagesState } from "../../state/ChatListWithMessagesState";
import nonAvatar from "../../assets/images/non_avatar.jpg";
import { signalRConfiguration } from "../../services/signalR/SignalRConfiguration";
import { SignalRMethodsName } from "../../models/enum/SignalRMethodsName";
import { motion } from "framer-motion";

const CreateChat = observer(() => {
  const [inputName, setInputName] = useState<string>("");
  const [inputTitle, setInputTitle] = useState<string>("");

  const [selectChatTypeValue, setSelectChatTypeValue] = useState<number>(1);

  const [file, setFile] = useState<File | null>();
  const [blobUrlFile, setBlobUrlFile] = useState<string | ArrayBuffer | null>();

  const setFileHandler = (event: React.FormEvent<HTMLInputElement>) => {
    const files = event.currentTarget.files;

    if (files && files.length > 0) {
      setFile(files[0]);

      const reader = new FileReader();
      reader.onload = () => {
        setBlobUrlFile(reader.result);
      };

      reader.readAsDataURL(files[0]);
    }
  };

  const onClickCreateChatHandler = async () => {

    const inputNameConst = inputName;
    const inputTitleConst = inputTitle;
    const selectChatTypeValueConst = selectChatTypeValue;
    const fileConst = file;

    setInputName("");
    setInputTitle("");
    setFile(null);
    setBlobUrlFile(null);

    const response = await chatListWithMessagesState.postCreateChatAsync(
      inputNameConst,
      inputTitleConst,
      selectChatTypeValueConst,
      fileConst ?? null
    );

    await signalRConfiguration.connection?.invoke(
      SignalRMethodsName.JoinChat,
      response.data.id
    );
  };

  return (
    <div className={styles.createChat}>
      <motion.div
        initial={{ y: -5 }}
        animate={{ y: 0 }}
        className={styles.avatarContainer}>
        <label htmlFor="avatar" className={styles.avatarBlackCover} />
        <input
          className={styles.avatarInput}
          onChange={setFileHandler}
          type="file"
          id="avatar"
          name="avatar"
          accept="image/jpeg"
        />
        <img className={styles.avatar} src={blobUrlFile?.toString() || nonAvatar} />
      </motion.div>
      <div className={styles.createChatContainer}>
        <input
          className={styles.inputTitle}
          type="text"
          placeholder="Name"
          value={inputName}
          onChange={(e) => setInputName(e.currentTarget.value)}
        />
        <input
          className={styles.inputName}
          type="text"
          placeholder="Title"
          value={inputTitle}
          onChange={(e) => setInputTitle(e.currentTarget.value)}
        />
        <select
          className={styles.chooseChatTypeSelect}
          onChange={(e) =>
            setSelectChatTypeValue(Number(e.currentTarget.value))
          }
        >
          <option value={ChatType.Conversation}>Conversation</option>
          <option value={ChatType.Channel}>Channel</option>
        </select>
        <button className={styles.CreateChatButton} onClick={onClickCreateChatHandler}>
          Create Chat
        </button>
      </div>
    </div>
  );
});

export default CreateChat;
