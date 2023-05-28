import React, { useState, useEffect } from "react";
import { observer } from "mobx-react-lite";
import styles from "./ProfileInfo.module.scss";
import { ReactComponent as SettingsSvg } from "../../assets/svg/three_dots.svg";
import { ReactComponent as CrossSvg } from "../../assets/svg/cross.svg";
import ProfileInfoBurgerMenu from "../profileInfoBurgerMenu/ProfileInfoBurgerMenu";
import { ReactComponent as TickSvg } from "../../assets/svg/tick_white.svg";
import { currentProfileState } from "../../state/CurrentProfileState";
import { authorizationState } from "../../state/AuthorizationState";
import { sessionsState } from "../../state/SessionsState";
import DateService from "../../services/messenger/DateService";
import nonAvatar from "../../assets/images/non_avatar.jpg";
import { chatListWithMessagesState } from "../../state/ChatListWithMessagesState";
import { signalRConfiguration } from "../../services/signalR/SignalRConfiguration";
import { SignalRMethodsName } from "../../models/enum/SignalRMethodsName";
import { blackCoverState } from "../../state/BlackCoverState";
import { motion } from "framer-motion";
import { ChatType } from "../../models/enum/ChatType";
import { currentChatState } from "../../state/CurrentChatState";
import TokenService from "../../services/messenger/TokenService";
import { useNavigate } from "react-router-dom";
import RouteConstants from "../../constants/RouteConstants";

const ProfileInfo = observer(() => {

  const isMyProfile = !currentProfileState.date;

  const displayName = isMyProfile ? authorizationState.data?.displayName ?? "" : currentProfileState.date?.displayName ?? "";
  const nickname = isMyProfile ? authorizationState.data?.nickname ?? "" : currentProfileState.date?.nickname ?? "";
  const bio = isMyProfile ? authorizationState.data?.bio ?? "" : currentProfileState.date?.bio ?? "";

  const [updateMode, setUpdateMode] = useState<boolean>(false);

  const [inputDisplayName, setInputDisplayName] = useState<string>("");
  const [inputNickname, setInputNickname] = useState<string>("");
  const [inputAdditionalData, setInputAdditionalData] = useState<string>("");

  const [showMenu, SetShowMenu] = useState<boolean>(false);
  const [showSessions, SetShowSessions] = useState<boolean>(false);

  const [xMousePosition, setXMousePosition] = useState<number>(0);
  const [yMousePosition, setYMousePosition] = useState<number>(0);

  const currentProfileStateDate = currentProfileState.date;
  const authorizationStateData =  authorizationState.data;

  const navigate = useNavigate();

  const currentProfileId = currentProfileState.date?.id;
  const dialogWithThisUser =
    chatListWithMessagesState.data.find(
      (x) => x.chat.type === ChatType.Dialog
        && x.chat.members.find(m => m.id !== authorizationState.data?.id)?.id === currentProfileId
    );

  const MouseMoveHandler = (event: React.MouseEvent<HTMLDivElement>) => {
    const localX = event.clientX - event.currentTarget.offsetLeft;
    const localY = event.clientY - event.currentTarget.offsetTop;

    setXMousePosition(localX);
    setYMousePosition(localY);
  };

  const onClickUpdateProfileDateHandler = async () => {
    try {
      await authorizationState.putUpdateProfileAsync(
        inputDisplayName,
        inputNickname,
        inputAdditionalData
      );
    } catch (error: any) {
      if (error.response.status !== 401) {
        alert(error.response.data.message);
      }
    }
    setUpdateMode(false);
  };

  const setShowSessionsHandler = async () => {
    SetShowSessions(!showSessions);

    if (sessionsState.data.length === 0) {
      try {
        await sessionsState.getSessionListAsync();
      } catch (error: any) {
        if (error.response.status !== 401) {
          alert(error.response.data.message);
        }
      }
    }
  };

  const deleteSessionHandler = async (sessionId: string) => {
    try {
      await sessionsState.delDeleteSessionAsync(sessionId);

      if (sessionId === authorizationState.data?.currentSessionId) {
        TokenService.deleteLocalAccessToken();
        TokenService.deleteLocalRefreshToken();
        return navigate(RouteConstants.Login, { replace: true });
      }
    } catch (error: any) {
      if (error.response.status !== 401) {
        alert(error.response.data.message);
      }
    }
  };

  const onClickStartChattingHandler = async () => {
    if (!currentProfileId) return;

    try {
      const response = await chatListWithMessagesState.postCreateDialogAsync(currentProfileId);

      const dialogDataItem = chatListWithMessagesState.data.find(x => x.chat.id === response.data.id);

      if (!dialogDataItem) return;

      currentChatState.setChatAndMessages(dialogDataItem.chat, dialogDataItem.messages);
      chatListWithMessagesState.pushChatOnTop(dialogDataItem.chat.id);

      if (response.status === 200) {
        await signalRConfiguration.connection?.invoke(SignalRMethodsName.JoinChat, response.data.id);
      }
    } catch (error: any) {
      if (error.response.status !== 401) {
        alert(error.response.data.message);
      }
    }
  };

  const onChangeAvatarHandler = async (
    event: React.FormEvent<HTMLInputElement>
  ) => {
    const files = event.currentTarget.files;

    try {
      if (files && files.length > 0) {
        await authorizationState.putUpdateProfileAvatarAsync(files[0]);
      }
    } catch (error: any) {
      if (error.response.status !== 401) {
        alert(error.response.data.message);
      }
    }
  };

  const onClickOpenFullSizeAvatar = () => {
    blackCoverState.setImage(currentProfileState.date?.avatarLink ?? nonAvatar);
  };

  useEffect(() => {
    setInputDisplayName(displayName);
    setInputNickname(nickname);
    setInputAdditionalData(bio);
    SetShowSessions(false);
  }, [currentProfileStateDate, authorizationStateData]);

  return (
    <div
      className={styles.profileInfo}
      onMouseMove={(event) => MouseMoveHandler(event)}
    >
      {
        updateMode && (
          <button className={styles.okButton} onClick={onClickUpdateProfileDateHandler}>
            <TickSvg width={15} height={23} />
          </button>
        )
      }

      {
        showMenu && (
          <ProfileInfoBurgerMenu
            x={xMousePosition}
            y={yMousePosition}
            profileId={currentProfileState.date?.id ?? ""}
            setShowMenu={SetShowMenu}
            setUpdateMode={setUpdateMode}
          />
        )
      }
      {
        ((dialogWithThisUser && !isMyProfile) || isMyProfile) &&
        <button
          className={styles.settingsButton}
          onClick={() => SetShowMenu(true)}
        >
          <SettingsSvg className={styles.settingsSvg} width={20} />
        </button>}
      <div className={styles.avatarContainer}>
        {
          isMyProfile && <label htmlFor="avatar" className={styles.avatarBlackCover} />
        }
        <input
          className={styles.avatarInput}
          onChange={onChangeAvatarHandler}
          type="file"
          id="avatar"
          name="avatar"
          accept="image/jpeg"
        />
        <img
          className={styles.avatar}
          src={
            currentProfileState.date
              ? currentProfileState.date.avatarLink || nonAvatar
              : authorizationState.data?.avatarLink || nonAvatar
          }
          onClick={onClickOpenFullSizeAvatar}
         alt={"avatar"}/>
      </div>
      {
        !updateMode && (
          <p className={styles.displayName}>{inputDisplayName || "------"}</p>
        )
      }
      {
        updateMode && (
          <input
            className={styles.inputDisplayName}
            type="text"
            value={inputDisplayName}
            onChange={(e) => setInputDisplayName(e.currentTarget.value)}
          />
        )
      }
      {
        !updateMode && (
          <p className={styles.nickname}>{inputNickname || "------"}</p>
        )
      }
      {
        updateMode && (
          <input
            className={styles.inputNickname}
            type="text"
            value={inputNickname}
            onChange={(e) => setInputNickname(e.currentTarget.value)}
          />
        )
      }
      {
        !updateMode && (
          <p className={styles.additionalData}>{inputAdditionalData}</p>
        )
      }
      {
        updateMode && (
          <input
            className={styles.inputAdditionalData}
            type="text"
            value={inputAdditionalData}
            onChange={(e) => setInputAdditionalData(e.currentTarget.value)}
          />
        )
      }
      {
        currentProfileState.date &&
        authorizationState.data?.id !== currentProfileState.date?.id &&
        !dialogWithThisUser && (
          <button
            className={styles.startChattingButton}
            onClick={onClickStartChattingHandler}
          >
            Start Chatting
          </button>
        )
      }
      {
        (currentProfileState.date?.id === authorizationState.data?.id ||
          !currentProfileState.date) && (
          <button
            className={styles.showMembersButton}
            onClick={() => setShowSessionsHandler()}
          >
            {showSessions ? "Hide" : "Show"} Sessions
          </button>
        )
      }
      {
        showSessions && (
          <div className={styles.sessionList}>
            {
              sessionsState.data.map((i) => (
                <motion.div
                  initial={{ opacity: 0.7 }}
                  animate={{ opacity: 1 }}
                  transition={{ duration: .1 }}
                  className={i.id === authorizationState.data?.currentSessionId ?
                    styles.currentSessionItem :
                    styles.sessionItem}
                  key={i.id}
                >
                  <div className={styles.sessionItemRemoveButton} onClick={() => deleteSessionHandler(i.id)}>
                    <CrossSvg width={20} />
                  </div>
                  <div className={styles.sessionItemContainer}>
                    <p className={styles.sessionId}>Id: {i.id}</p>
                    <p className={styles.createAt}>
                      CreateAt: {DateService.getDateAndTime(i.createAt)}
                    </p>
                  </div>
                </motion.div>
              ))
            }
          </div>
        )
      }
    </div>
  );
});

export default ProfileInfo;
