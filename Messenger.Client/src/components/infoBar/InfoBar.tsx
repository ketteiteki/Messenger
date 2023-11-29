import React from "react";
import { observer } from "mobx-react-lite";
import styles from "./InfoBar.module.scss";
import { Outlet, useNavigate } from "react-router-dom";
import nonAvatar from "../../assets/images/non_avatar.jpg";
import { authorizationState } from "../../state/AuthorizationState";
import { currentProfileState } from "../../state/CurrentProfileState";
import { motion } from "framer-motion";
import RouteConstants from "../../constants/RouteConstants";

const InfoBar = observer(() => {
  const navigate = useNavigate();

  const showMyProfileHandler = () => {
    currentProfileState.setProfileNull();
    return navigate(RouteConstants.Layout, { replace: true });
  };

  return (
    <div className={styles.chatInfo}>
      <div className={styles.header}>
        <motion.p
          initial={{ y: -5 }}
          animate={{ y: 0 }}
          className={styles.headerDisplayName}
          onClick={showMyProfileHandler}>
          {
            authorizationState.data?.displayName
          }
        </motion.p>
        <img
          className={styles.headerAvatar}
          src={authorizationState.data?.avatarLink ?? nonAvatar
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
