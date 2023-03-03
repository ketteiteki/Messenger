import React from "react";
import { observer } from "mobx-react-lite";
import styles from "./InfoBar.module.scss";
import { Outlet, useNavigate } from "react-router-dom";
import nonAvatar from "../../assets/images/non_avatar.jpg";
import { authorizationState } from "../../state/AuthorizationState";
import { currentProfileState } from "../../state/CurrentProfileState";

const InfoBar = observer((props) => {
  const navigate = useNavigate();

  const showMyProfileHandler = () => {
    currentProfileState.setProfileNull();
    navigate("/", { replace: true });
  };

  return (
    <div className={styles.chatInfo}>
      <div className={styles.header}>
        <p className={styles.headerDisplayName} onClick={showMyProfileHandler}>
          {authorizationState.data?.displayName}
        </p>
        <img
          className={styles.headerAvatar}
          src={
            authorizationState.data?.avatarLink !== null
              ? authorizationState.data?.avatarLink
              : nonAvatar
          }
          alt=""
          onClick={showMyProfileHandler}
        />
      </div>
      <Outlet />
    </div>
  );
});

export default InfoBar;
