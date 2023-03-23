import React, { useCallback, useState, useEffect } from "react";
import { observer } from "mobx-react-lite";
import styles from "./ChatList.module.scss";
import { ReactComponent as MessegnerLogoSvg } from "../../assets/svg/messenger_logo.svg";
import { ReactComponent as LoupeSvg } from "../../assets/svg/loupe.svg";
import { ReactComponent as PenSvg } from "../../assets/svg/pen.svg";
import ChatListItem from "../chatListItem/ChatListItem";
import { chatListWithMessagesState } from "../../state/ChatListWithMessagesState";
import { Link, useNavigate } from "react-router-dom";
import { useDebouncedCallback } from "use-debounce";
import { signalRConfiguration } from "../../services/signalR/SignalRConfiguration";
import { SignalRMethodsName } from "../../models/enum/SignalRMethodsName";
import { currentChatState } from "../../state/CurrentChatState";

const ChatList = observer(() => {
  const [inputSearch, setInputSearch] = useState<string>("");

  const navigate = useNavigate();

  const debounced = useDebouncedCallback(
    useCallback(async (value) => {
      if (value === "") return;

      const response = await chatListWithMessagesState.getChatListBySearchAsync(
        value
      );

      if (response.status === 200) {
        response.data.forEach(async (c) => {
          await signalRConfiguration.connection?.invoke(
            SignalRMethodsName.JoinChat,
            c.id
          );
        });
      }
    }, []),
    700,
    { maxWait: 2000 }
  );

  const searchChatListHandler = (value: string) => {
    setInputSearch(value);
    debounced(value);
  };

  useEffect(() => {
    if (inputSearch === "") {
      const dataChats = chatListWithMessagesState.data;
      const dataForSearchChats = chatListWithMessagesState.dataForSearchChats;

      dataForSearchChats.forEach(async (i) => {
        if (dataChats.find((x) => x.chat.id === i.chat.id)) return;
        await signalRConfiguration.connection?.invoke(
          SignalRMethodsName.LeaveChat,
          i.chat.id
        );
      });

      currentChatState.setChatAndMessagesNull();

      navigate("/", { replace: true });
    }
  }, [inputSearch]);

  return (
    <div className={styles.chatList}>
      <div className={styles.header}>
        <MessegnerLogoSvg width={30} />
        <p>Messenger</p>
        <Link to={"/createChat"} className={styles.createChatButton}>
          <PenSvg className={styles.penSvg} width={25} />
        </Link>
      </div>
      <div className={styles.search}>
        <LoupeSvg width={18} />
        <input
          type="text"
          placeholder="Search"
          defaultValue={""}
          onChange={(e) => searchChatListHandler(e.currentTarget.value)}
        />
      </div>
      <div className={styles.items}>
        {inputSearch === "" &&
          chatListWithMessagesState.data.map((i) => (
            <ChatListItem key={i.chat.id} {...i} />
          ))}
        {inputSearch !== "" &&
          chatListWithMessagesState.dataForSearchChats.map((i) => (
            <ChatListItem key={i.chat.id} {...i} />
          ))}
      </div>
    </div>
  );
});

export default ChatList;
