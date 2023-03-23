import React, { useState } from "react";
import { observer } from "mobx-react-lite";
import styles from "./ChatInfoBurgerMenu.module.scss";
import { ReactComponent as EditSvg } from "../../assets/svg/edit_black.svg";
import { ReactComponent as TrashBinSvg } from "../../assets/svg/trash_bin_black.svg";
import { chatListWithMessagesState } from "../../state/ChatListWithMessagesState";
import { currentChatState } from "../../state/CurrentChatState";
import { useNavigate } from "react-router-dom";

interface ProfileInfoBurgerMenu {
  x: number;
  y: number;
  setShowMenu: React.Dispatch<React.SetStateAction<boolean>>;
  setUpdateMode: React.Dispatch<React.SetStateAction<boolean>>;
}

const ChatInfoBurgerMenu = observer(
  ({ x, y, setShowMenu, setUpdateMode }: ProfileInfoBurgerMenu) => {
    const [xMousePosition, setXMousePosition] = useState<number>(x - 100);
    const [yMousePosition, setYMousePosition] = useState<number>(y);

    const navigate = useNavigate();

    const deleteChatHandler = async () => {
      await chatListWithMessagesState.delDeleteChatAsync(
        currentChatState.chat?.id ?? ""
      );
      currentChatState.setChatAndMessagesNull();

      navigate("/", { replace: true });
    };

    return (
      <div
        className={styles.chatInfoBurgerMenu}
        style={{ left: `${xMousePosition}px`, top: `${yMousePosition}px` }}
        onMouseLeave={() => setShowMenu(false)}
      >
        <button
          className={styles.changeChatDataButton}
          onClick={() => setUpdateMode(true)}
        >
          <EditSvg width={20} />
          <p>Edit</p>
        </button>
        <button className={styles.deleteChatButton} onClick={deleteChatHandler}>
          <TrashBinSvg width={20} />
          <p>Delete</p>
        </button>
      </div>
    );
  }
);

export default ChatInfoBurgerMenu;
