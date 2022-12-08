import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router";
import { useAppDispatch, useAppSelector } from "../app/hooks";
import { selectAuthorization } from "../app/slices/authorizationSlice";
import { loginAsync } from "../app/thunks/authorizationThucks";
import { RequestStatusEnum } from "../models/enums/RequestStatusEnum";
import TokenService from "../services/messenger/TokenService";

export const Login = () => {
  const authorizationState = useAppSelector(selectAuthorization);
  const dispatch = useAppDispatch();

  const navigate = useNavigate();

  const [nickname, setNickname] = useState<string>("");
  const [password, setPassword] = useState<string>("");

  useEffect(() => {
    TokenService.deleteLocalAccessToken();
    TokenService.deleteLocalRefreshToken();
  }, [])

  useEffect(() => {
    if (authorizationState.status == RequestStatusEnum.success) {
      navigate("/", {replace: true});
    }
  }, [authorizationState.status])

  const onClickLogin = (e: React.MouseEvent) => {
    e.preventDefault();

    dispatch(loginAsync({ nickname, password }));
  };

  return (
    <div className="login">
      <div className="login__background"></div>
      <form className="login__form">
        <input
          className="login__input_nickname"
          placeholder="Nickname"
          type="text"
          value={nickname}
          onChange={(e) => setNickname(e.currentTarget.value)}
        />
        <input
          className="login__input_password"
          placeholder="Password"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.currentTarget.value)}
        />
        <button className="login__button_log-in" onClick={onClickLogin}>
          Log In
        </button>
      </form>
    </div>
  );
};
