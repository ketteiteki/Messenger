import React, { useEffect, useState } from "react";
import { observer } from "mobx-react-lite";
import styles from "./ChatInfo.module.scss";
import { ReactComponent as SettingsSvg } from "../../assets/svg/three_dots.svg";
import { ReactComponent as TickSvg } from "../../assets/svg/tick_white.svg";
import ChatInfoBurgerMenu from "../chatInfoBurgerMenu/ChatInfoBurgerMenu";
import nonAvatar from "../../assets/images/non_avatar.jpg";
import { currentChatState } from "../../state/CurrentChatState";
import { chatListWithMessagesState } from "../../state/ChatListWithMessagesState";
import { useNavigate } from "react-router-dom";
import { blackCoverState } from "../../state/BlackCoverState";

const ChatInfo = observer(() => {
  const [updateMode, setUpdateMode] = useState<boolean>(false);

  const [inputTitle, setInputTitle] = useState<string>();
  const [inputName, setInputName] = useState<string>();

  const [showMenu, SetShowMenu] = useState<boolean>(false);
  const [showMemberList, setShowMemberList] = useState<boolean>(false);

  const [xMousePosition, setXMousePosition] = useState<number>(0);
  const [yMousePosition, setYMousePosition] = useState<number>(0);

  const navigate = useNavigate();

  const currentChatId = currentChatState.chat?.id;
  const isMyChat = currentChatState.chat?.isOwner;

  const mouseMoveHandler = (event: React.MouseEvent<HTMLDivElement>) => {
    const localX = event.clientX - event.currentTarget.offsetLeft;
    const localY = event.clientY - event.currentTarget.offsetTop;

    setXMousePosition(localX);
    setYMousePosition(localY);
  };

  const onClickLeaveChatHandler = async () => {
    currentChatState.chat &&
      (await chatListWithMessagesState.postLeaveFromChatAsync(
        currentChatState.chat.id
      ));
    currentChatState.setChatAndMessagesNull();

    return navigate("/", { replace: true });
  };

  const updateChatHandler = async () => {
    const currentChatId = currentChatState.chat?.id;

    const response = await chatListWithMessagesState.putUpdateChatDataAsync(
      currentChatId ?? "",
      inputName ?? "",
      inputTitle ?? ""
    );

    if (response.status === 200) {
      currentChatState.updateChatByChat(response.data);
    }

    setUpdateMode(false);
  };

  const onClickShowMemberListHandler = async () => {
    if (currentChatState.chat === null) return;

    setShowMemberList(!showMemberList);

    if (currentChatState.chat.members.length === 0) {
      await currentChatState.getUserListByChatAsync(
        currentChatState.chat.id,
        20,
        1
      );
    }
  };

  const onChangeAvatarHandler = async (
    event: React.FormEvent<HTMLInputElement>
  ) => {
    const files = event.currentTarget.files;

    const currentChatId = currentChatState.chat?.id;

    if (currentChatId === undefined) return;

    if (files && files.length > 0) {
      await chatListWithMessagesState.putUpdateChatAvatarAsync(
        currentChatId,
        files[0]
      );
    }
  };

  const onClickJoinChatHandler = async () => {

    if (currentChatId === undefined || currentChatState.chat === null) return;

    const response = await currentChatState.postJoinToChatAsync(currentChatId);

    chatListWithMessagesState.addChatInData(currentChatState.chat, currentChatState.messages)
    chatListWithMessagesState.resetDataForSearchChats();
    currentChatState.setChatAndMessages(currentChatState.chat, currentChatState.messages);
  }

  const onClickOpenFullSizeAvatar = () => {
    blackCoverState.setImage(currentChatState.chat?.avatarLink ?? nonAvatar);
  };

  useEffect(() => {
    setInputTitle(currentChatState.chat?.title ?? "");
    setInputName(currentChatState.chat?.name ?? "");
    SetShowMenu(false);
    setShowMemberList(false);
    setUpdateMode(false);
  }, [currentChatState.chat]);

  return (
    <div
      className={styles.chatInfo}
      onMouseMove={(event) => mouseMoveHandler(event)}
    >
      {updateMode && (
        <button className={styles.okButton} onClick={updateChatHandler}>
          <TickSvg width={15} height={23} />
        </button>
      )}
      {showMenu && (
        <ChatInfoBurgerMenu
          x={xMousePosition}
          y={yMousePosition}
          setShowMenu={SetShowMenu}
          setUpdateMode={setUpdateMode}
        />
      )}
      {currentChatState.chat?.isOwner && (
        <button
          className={styles.settingsButton}
          onClick={() => SetShowMenu(true)}
        >
          <SettingsSvg className={styles.settingsSvg} width={20} />
        </button>
      )}
      <div className={styles.avatarContainer}>
        {isMyChat && <label htmlFor="avatar" className={styles.avatarBlackCover} />}
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
          src={currentChatState.chat?.avatarLink ?? nonAvatar}
          alt=""
          onClick={onClickOpenFullSizeAvatar}
        />
      </div>
      {updateMode === false && <p className={styles.title}>{inputTitle}</p>}
      {updateMode && (
        <input
          className={styles.inputTitle}
          type="text"
          value={inputTitle}
          onChange={(e) => setInputTitle(e.currentTarget.value)}
        />
      )}
      {updateMode === false && <p className={styles.name}>{inputName}</p>}
      {updateMode && (
        <input
          className={styles.inputName}
          type="text"
          value={inputName}
          onChange={(e) => setInputName(e.target.value)}
        />
      )}
      {chatListWithMessagesState.data.find(
        (x) => x.chat.id === currentChatState.chat?.id
      ) === undefined ? (
        <button className={styles.joinChattingButton} onClick={onClickJoinChatHandler}>Join</button>
      ) : (
        <button
          className={styles.leaveChattingButton}
          onClick={onClickLeaveChatHandler}
        >
          Leave
        </button>
      )}

      <button
        className={styles.showMembersButton}
        onClick={onClickShowMemberListHandler}
      >
        {showMemberList ? "Close" : "Show"} Members
      </button>
      {showMemberList && (
        <div className={styles.memberList}>
          {currentChatState.chat?.members.map((i) => (
            <div className={styles.memberItem} key={i.id}>
              <img
                className={styles.memberItemAvatar}
                src={i.avatarLink ?? nonAvatar}
                alt=""
              />
              <div className={styles.memberItemContainer}>
                <p className={styles.memberItemDisplayName}>{i.displayName}</p>
                <p className={styles.memberItemBio}>{i.bio}</p>
                <p className={styles.memberItemRole}>
                  {
                    currentChatState.chat?.usersWithRole.find(
                      (u) => u.userId === i.id
                    )?.roleTitle
                  }
                </p>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
});

export default ChatInfo;
