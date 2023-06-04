import React, { useCallback, useEffect } from "react";
import { observer } from "mobx-react-lite";
import styles from "./ChatList.module.scss";
import { ReactComponent as MessengerLogoSvg } from "../../assets/svg/messenger_logo.svg";
import { ReactComponent as SearchSvg } from "../../assets/svg/loupe.svg";
import { ReactComponent as PenSvg } from "../../assets/svg/pen.svg";
import ChatListItem from "../chatListItem/ChatListItem";
import { chatListWithMessagesState } from "../../state/ChatListWithMessagesState";
import { Link } from "react-router-dom";
import { useDebouncedCallback } from "use-debounce";
import { signalRConfiguration } from "../../services/signalR/SignalRConfiguration";
import { SignalRMethodsName } from "../../models/enum/SignalRMethodsName";
import { motion } from "framer-motion";

const ChatList = observer(() => {
  const searchInput = chatListWithMessagesState.searchInput;

  const debounced = useDebouncedCallback(
    useCallback(async (value) => {
      if (!value) return;

      const response = await chatListWithMessagesState
        .getChatListBySearchAsync(value)
        .catch((error: any) => { if (error.response.status !== 401) alert(error.response.data.message); });

      if (response && response.status === 200) {
        response.data.forEach((c) => {
          signalRConfiguration.connection?.invoke(SignalRMethodsName.JoinChat, c.id);
        });
      }
    }, []),
    700,
    { maxWait: 2000 }
  );

  const searchChatListHandler = (value: string) => {
    chatListWithMessagesState.setSearchInput(value);
    debounced(value);
  };

  useEffect(() => {
    if (!searchInput) {
      const dataChats = chatListWithMessagesState.data;
      const dataForSearchChats = chatListWithMessagesState.dataForSearchChats;

      dataForSearchChats.forEach(async (i) => {
        if (dataChats.find((x) => x.chat.id === i.chat.id)) return;

        await signalRConfiguration.connection?.invoke(SignalRMethodsName.LeaveChat, i.chat.id);
      });
    }
  }, [searchInput]);

  return (
    <div className={styles.chatList}>
      <div className={styles.header}>
        <MessengerLogoSvg width={30} />
        <motion.p initial={{ y: -5 }} animate={{ y: 0 }}
        >Messenger</motion.p>
        <Link to={"/createChat"} className={styles.createChatButton}>
          <PenSvg className={styles.penSvg} width={25} />
        </Link>
      </div>
      <div className={styles.search}>
        <SearchSvg width={18} />
        <input
          type="text"
          placeholder="Search"
          defaultValue={""}
          value={searchInput}
          onChange={(e) => searchChatListHandler(e.currentTarget.value)}
        />
      </div>
      <div className={styles.items}>
        {
          !searchInput &&
          chatListWithMessagesState.data.map((i) => (
            <ChatListItem key={i.chat.id} {...i} />
          ))
        }
        {
          searchInput &&
          chatListWithMessagesState.dataForSearchChats.map((i) => (
            <ChatListItem key={i.chat.id} {...i} />
          ))
        }
      </div>
    </div>
  );
});

export default ChatList;
