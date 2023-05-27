import React, { useEffect, useState } from "react";
import { observer } from "mobx-react-lite";
import styles from "./Login.module.scss";
import { authorizationState } from "../../state/AuthorizationState";
import { useNavigate, Link } from "react-router-dom";
import TokenService from "../../services/messenger/TokenService";
import { chatListWithMessagesState } from "../../state/ChatListWithMessagesState";
import { sessionsState } from "../../state/SessionsState";
import { signalRConfiguration } from "../../services/signalR/SignalRConfiguration";
import { currentChatState } from "../../state/CurrentChatState";

const Login = observer(() => {
  const [inputNickname, setInputNickname] = useState<string>("");
  const [inputPassword, setInputPassword] = useState<string>("");

  const navigate = useNavigate();

  useEffect(() => {
    TokenService.deleteLocalAccessToken();
    TokenService.deleteLocalRefreshToken();
    authorizationState.clearAuthorizationData();
    chatListWithMessagesState.clearChatListWithMessagesData();
    currentChatState.clearChatAndMessages();
    sessionsState.clearData();
    signalRConfiguration.connection?.stop();
  }, []);

  const loginHandler = async () => {
    try {
      const response = await authorizationState.postLoginAsync(
        inputNickname,
        inputPassword
      );

      if (response.status === 200) {
        return navigate("/", { replace: true });
      }
    } catch (error: any) {
      alert(error.response.data.message);
    }
  };

  const onEnterLoginHandler = async (
    event: React.KeyboardEvent<HTMLInputElement>
  ) => {
    if (event.key === "Enter") {
      const response = await authorizationState.postLoginAsync(
        inputNickname,
        inputPassword
      );

      if (response.status === 200) {
        return navigate("/", { replace: true });
      }
    }
  };

  return (
    <div className={styles.login}>
      <div className={styles.background} />
      <div className={styles.loginContainer}>
        <div className={styles.loginData}>
          <input
            className={styles.inputNickname}
            type="text"
            placeholder="Nickname"
            value={inputNickname}
            onKeyDown={onEnterLoginHandler}
            onChange={(e) => setInputNickname(e.currentTarget.value)}
          />
          <input
            className={styles.inputPassword}
            type="password"
            placeholder="Password"
            value={inputPassword}
            onKeyDown={onEnterLoginHandler}
            onChange={(e) => setInputPassword(e.currentTarget.value)}
          />
          <button className={styles.LoginButton} onClick={loginHandler}>
            Log In
          </button>
          <div className={styles.registerPage}>
            Don't have an account?{" "}
            <Link className={styles.registerPageLink} to={"/registration"}>
              Register
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
});

export default Login;
