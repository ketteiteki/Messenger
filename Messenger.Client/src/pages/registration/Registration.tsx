import React, { useEffect, useState } from "react";
import { observer } from "mobx-react-lite";
import styles from "./Registration.module.scss";
import { authorizationState } from "../../state/AuthorizationState";
import { useNavigate, Link } from "react-router-dom";
import { chatListWithMessagesState } from "../../state/ChatListWithMessagesState";
import { sessionsState } from "../../state/SessionsState";
import { signalRConfiguration } from "../../services/signalR/SignalRConfiguration";
import { currentChatState } from "../../state/CurrentChatState";
import RouteConstants from "../../constants/RouteConstants";
import {currentProfileState} from "../../state/CurrentProfileState";

const Registration = observer(() => {
  const [inputDisplayName, setInputDisplayName] = useState<string>("");
  const [inputNickname, setInputNickname] = useState<string>("");
  const [inputPassword, setInputPassword] = useState<string>("");

  const navigate = useNavigate();

  useEffect(() => {
    authorizationState.clearAuthorizationData();
    chatListWithMessagesState.clearChatListWithMessagesData();
    currentChatState.clearChatAndMessages();
    currentProfileState.setProfileNull()
    sessionsState.clearData();
    signalRConfiguration.connection?.stop();
  }, []);

  const registerHandler = async () => {
    try {
      const response = await authorizationState.postRegistrationAsync(
        inputDisplayName,
        inputNickname,
        inputPassword
      );

      if (response.status === 200) {
        return navigate(RouteConstants.Layout, { replace: true });
      }
    } catch (error: any) {
      alert(error.response.data.message);
    }
  };

  const onEnterRegisterHandler = async (
    event: React.KeyboardEvent<HTMLInputElement>
  ) => {
    if (event.key === "Enter") {
      const response = await authorizationState.postLoginAsync(
        inputNickname,
        inputPassword
      );

      if (response.status === 200) {
        return navigate(RouteConstants.Layout, { replace: true });
      }
    }
  };

  return (
    <div className={styles.registration}>
      <div className={styles.background} />
      <div className={styles.registrationContainer}>
        <div className={styles.registrationData}>
          <input
            className={styles.inputDisplayName}
            type="text"
            placeholder="DisplayName"
            value={inputDisplayName}
            onKeyDown={onEnterRegisterHandler}
            onChange={(e) => setInputDisplayName(e.currentTarget.value)}
          />
          <input
            className={styles.inputNickname}
            type="text"
            placeholder="Nickname"
            value={inputNickname}
            onKeyDown={onEnterRegisterHandler}
            onChange={(e) => setInputNickname(e.currentTarget.value)}
          />
          <input
            className={styles.inputPassword}
            type="password"
            placeholder="Password"
            value={inputPassword}
            onKeyDown={onEnterRegisterHandler}
            onChange={(e) => setInputPassword(e.currentTarget.value)}
          />
          <button
            className={styles.registrationButton}
            onClick={registerHandler}
          >
            Register
          </button>
          <div className={styles.loginPage}>
            Do you already have an account?{" "}
            <Link className={styles.loginPageLink} to={RouteConstants.Login}>
              Login
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
});

export default Registration;
