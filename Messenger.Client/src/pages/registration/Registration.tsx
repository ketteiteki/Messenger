import React, { useEffect, useState } from "react";
import { observer } from "mobx-react-lite";
import styles from "./Registration.module.scss";
import { authorizationState } from "../../state/AuthorizationState";
import { useNavigate, Link } from "react-router-dom";
import TokenService from "../../services/messenger/TokenService";

const Registration = observer(() => {
  const [inputDisplayName, setInputDisplayName] = useState<string>("");
  const [inputNickname, setInputNickname] = useState<string>("");
  const [inputPassword, setInputPassword] = useState<string>("");

  const navigate = useNavigate();

  useEffect(() => {
    TokenService.deleteLocalAccessToken();
    TokenService.deleteLocalRefreshToken();
  }, []);

  const registerHandler = async () => {
    var response = await authorizationState.postRegistrationAsync(
      inputDisplayName,
      inputNickname,
      inputPassword
    );

    if (response.status === 200) {
      return navigate("/", { replace: true });
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
        return navigate("/", { replace: true });
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
            type="text"
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
            <Link className={styles.loginPageLink} to={"/login"}>
              Login
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
});

export default Registration;
