import axios from "axios";
import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router";
import { Link } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../app/hooks";
import { selectAuthorization } from "../app/slices/authorizationSlice";
import { registrationAsync } from "../app/thunks/authorizationThucks";
import { RequestStatusEnum } from "../models/enums/RequestStatusEnum";
import TokenService from "../services/messenger/TokenService";

export const Registration = () => {
  const authorizationState = useAppSelector(selectAuthorization);
  const dispatch = useAppDispatch();

  const navigate = useNavigate();

  const [displayName, setDisplayName] = useState<string>("");
  const [nickname, setNickname] = useState<string>("");
  const [password, setPassword] = useState<string>("");

  useEffect(() => {
    TokenService.deleteLocalAccessToken();
    TokenService.deleteLocalRefreshToken();
  }, [])

  useEffect(() => {
    if (authorizationState.status == RequestStatusEnum.success) {
      navigate("/", { replace: true });
    }
  }, [authorizationState.status]);

  const onClickRegistration = (e: React.MouseEvent) => {
    e.preventDefault();

    dispatch(
      registrationAsync({
        displayName,
        nickname,
        password,
      })
    );
  };

  return (
    <div className="registration">
      <div className="registration__background" />
      <form className="registration__form">
        <input
          className="registration__input_displayname"
          placeholder="Display name"
          type="text"
          value={displayName}
          onChange={(e) => setDisplayName(e.currentTarget.value)}
        />
        <input
          className="registration__input_nickname"
          placeholder="Nickname"
          type="text"
          value={nickname}
          onChange={(e) => setNickname(e.currentTarget.value)}
        />
        <input
          className="registration__input_password"
          placeholder="Password"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.currentTarget.value)}
        />
        <button
          className="registration__button_log-in"
          onClick={onClickRegistration}
        >
          Sign up
        </button>
        <p className="registration__form__page">
          Already have an account?{" "}
          <Link className="registration__form__page__link" to={"/login"}>
            Login
          </Link>
          .
        </p>
      </form>
    </div>
  );
};
