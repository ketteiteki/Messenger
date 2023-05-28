import React, { useEffect } from "react";
import { observer } from "mobx-react-lite";
import styles from "./Layout.module.scss";
import ChatList from "../../components/chatList/ChatList";
import Chat from "../../components/chat/Chat";
import InfoBar from "../../components/infoBar/InfoBar";
import { useNavigate } from "react-router-dom";
import { authorizationState } from "../../state/AuthorizationState";
import { chatListWithMessagesState } from "../../state/ChatListWithMessagesState";
import IMessageDto from "../../models/interfaces/IMessageDto";
import IMessageUpdateNotificationDto from "../../models/interfaces/IMessageUpdateNotificationDto";
import IMessageDeleteNotificationDto from "../../models/interfaces/IMessageDeleteNotificationDto";
import { signalRConfiguration } from "../../services/signalR/SignalRConfiguration";
import { SignalRMethodsName } from "../../models/enum/SignalRMethodsName";
import { blackCoverState } from "../../state/BlackCoverState";
import { motion } from "framer-motion";
import IChatDto from "../../models/interfaces/IChatDto";
import { currentChatState } from "../../state/CurrentChatState";
import ModalWindow from "../../components/modalWindow/ModalWindow";
import RouteConstants from "../../constants/RouteConstants";
import TokenService from "../../services/messenger/TokenService";

const Layout = observer(() => {
  const navigate = useNavigate();

  const authorizationStateCountFailRefresh = authorizationState.countFailRefresh;

  useEffect(() => {
    if (authorizationState.isRefreshFail) {
      // authorizationState.resetCountFailRefresh();
      return navigate(RouteConstants.Login, { replace: true });
    }

  }, [authorizationState.isRefreshFail]);

  useEffect(() => {
    const accessToken = localStorage.getItem("AuthorizationToken");
    const fun = async () => {
      if (accessToken == null) {
        return navigate(RouteConstants.Registration, { replace: true });
      }

      const authorizationResponse = await authorizationState.getAuthorizationAsync();

      if (authorizationResponse.status !== 200) {
        return navigate(RouteConstants.Registration, { replace: true });
      }

      TokenService.setLocalAccessToken(authorizationResponse.data.accessToken);
      TokenService.setLocalRefreshToken(authorizationResponse.data.refreshToken);

      const response = await chatListWithMessagesState.getChatListAsync();

      signalRConfiguration.buildConnection(authorizationResponse.data.accessToken);

      if (!signalRConfiguration.connection) return;
      signalRConfiguration.connection.on(SignalRMethodsName.BroadcastMessageAsync, (message: IMessageDto) => {
        if (message.ownerId === authorizationResponse.data.id) return;

        message.isMessageRealtime = true;
        chatListWithMessagesState.addMessageInData(message);
        chatListWithMessagesState.setLastMessage(message);
        chatListWithMessagesState.pushChatOnTop(message.chatId);
      });

      signalRConfiguration.connection.on(SignalRMethodsName.UpdateMessageAsync, (message: IMessageUpdateNotificationDto) => {
        if (message.ownerId === authorizationResponse.data.id) return;

        chatListWithMessagesState.updateMessageInData(message);
      });

      signalRConfiguration.connection.on(SignalRMethodsName.DeleteMessageAsync, (message: IMessageDeleteNotificationDto) => {
        if (message.ownerId === authorizationResponse.data.id) return;

        chatListWithMessagesState.deleteMessageInDataByMessageDeleteNotification(message);
      });

      signalRConfiguration.connection.on(SignalRMethodsName.CreateDialogForInterlocutor, async (chat: IChatDto) => {
        await signalRConfiguration.connection?.invoke(SignalRMethodsName.JoinChat, chat.id);
        chatListWithMessagesState.addChatInData(chat, []);
        chatListWithMessagesState.pushChatOnTop(chat.id);
      });

      signalRConfiguration.connection.on(SignalRMethodsName.DeleteDialogForInterlocutor, (chatId: string) => {
        chatListWithMessagesState.deleteChatInDataById(chatId);
        currentChatState.setChatAndMessagesNull();
      });

      signalRConfiguration.connection
        .start()
        .then(function () {
          response.data.forEach(async (c) => {
            await signalRConfiguration.connection?.invoke(SignalRMethodsName.JoinChat, c.id);
          });
        })
        .catch(function (err) {
          console.log(err);
        });
    };

    fun();
  }, []);

  return (
    <div className={styles.layout}>
      {
        blackCoverState.isBlackCoverShown &&
        blackCoverState.imageLink &&
        <motion.div
          initial={{ opacity: .7 }}
          animate={{ opacity: 1 }}
          className={styles.blackCover}
          onClick={() => blackCoverState.closeBlackCover()}>
          <motion.img
            initial={{ y: -5 }}
            animate={{ y: 0 }}
            className={styles.blackCoverImage}
            src={blackCoverState.imageLink} onClick={(e) => e.stopPropagation()} />
        </motion.div>
      }
      {
        blackCoverState.isBlackCoverShown &&
        blackCoverState.modalWindow &&
        <motion.div
          initial={{ opacity: .7 }}
          animate={{ opacity: 1 }}
          className={styles.blackCover}
          onClick={() => blackCoverState.closeBlackCover()}>
          <ModalWindow />
        </motion.div>
      }
      <div className={styles.background} />
      <div className={styles.layoutContainer}>
        <ChatList />
        <Chat />
        <InfoBar />
      </div>
    </div>
  );
});

export default Layout;
