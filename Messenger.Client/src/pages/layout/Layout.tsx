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

const Layout = observer(() => {

  const navigate = useNavigate();

  useEffect(() => {
    if (authorizationState.countFailRefresh >= 2) {
      authorizationState.resetCountFailRefresh();
      return navigate("/login", { replace: true });
    }

  }, [authorizationState.countFailRefresh]);

  useEffect(() => {
    const accessToken = localStorage.getItem("AuthorizationToken");

    const fun = async () => {
      if (accessToken == null) {
        return navigate("/registration", { replace: true });
      }

      const authorizationResponse = await authorizationState.getAuthorizationAsync(accessToken);

      if (authorizationResponse.status !== 200) {
        return navigate("/registration", { replace: true });
      }

      const response = await chatListWithMessagesState.getChatListAsync();

      signalRConfiguration.buildConnection(accessToken);

      if (signalRConfiguration.connection === null) return;

      signalRConfiguration.connection.on(SignalRMethodsName.BroadcastMessageAsync, (message: IMessageDto) => {
        if (message.ownerId === authorizationResponse.data.id) return;

        message.isMessageRealtime = true;
        chatListWithMessagesState.addMessageInData(message);
        chatListWithMessagesState.setLastMessage(message);
      });

      signalRConfiguration.connection.on(SignalRMethodsName.UpdateMessageAsync, (message: IMessageUpdateNotificationDto) => {
        if (message.ownerId === authorizationResponse.data.id) return;

        chatListWithMessagesState.updateMessageInData(message);
      });

      signalRConfiguration.connection.on(SignalRMethodsName.DeleteMessageAsync, (message: IMessageDeleteNotificationDto) => {
        if (message.ownerId === authorizationResponse.data.id) return;

        chatListWithMessagesState.deleteMessageInDataByMessageDeleteNotification(message);
      });

      signalRConfiguration.connection.on("CreateDialogForInterlocutor", (message: string) => {
        console.log(message);
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
      {blackCoverState.isBlackCoverShown && <div className={styles.blackCover} onClick={() => blackCoverState.closeBlackCover()}>
        <img
          className={styles.blackCoverImage}
          src={blackCoverState.imageLink ?? ""} onClick={(e) => e.stopPropagation()} />
      </div>}
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
