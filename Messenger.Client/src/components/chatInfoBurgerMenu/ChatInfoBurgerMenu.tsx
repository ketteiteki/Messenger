import React, { useState } from "react";
import { observer } from "mobx-react-lite";
import styles from "./ChatInfoBurgerMenu.module.scss";
import { ReactComponent as EditSvg } from "../../assets/svg/edit_black.svg";
import { ReactComponent as TrashBinSvg } from "../../assets/svg/trash_bin_black.svg";
import { chatListWithMessagesState } from "../../state/ChatListWithMessagesState";
import { currentChatState } from "../../state/CurrentChatState";
import { useNavigate } from "react-router-dom";
import { blackCoverState } from "../../state/BlackCoverState";
import ModalWindowEntity from "../../models/entities/ModalWindowEntity";
import RouteConstants from "../../constants/RouteConstants";

interface ProfileInfoBurgerMenu {
  x: number;
  y: number;
  setShowMenu: React.Dispatch<React.SetStateAction<boolean>>;
  setUpdateMode: React.Dispatch<React.SetStateAction<boolean>>;
}

const ChatInfoBurgerMenu = observer(
  ({ x, y, setShowMenu, setUpdateMode }: ProfileInfoBurgerMenu) => {
    const [xMousePosition] = useState<number>(x - 100);
    const [yMousePosition] = useState<number>(y);

    const navigate = useNavigate();

    const onClickDeleteChatHandler = async () => {

      const text = "Are you sure you want to delete the chat?";

      const okFun = async () => {
        try {
          await chatListWithMessagesState.delDeleteChatAsync(currentChatState.chat?.id ?? "");
        } catch (error: any) {
            if (error.response.status !== 401) {
                alert(error.response.data.message);
            }
        }

        currentChatState.setChatAndMessagesNull();
        return navigate(RouteConstants.Layout, { replace: true });
      }

      const modalWindowEntity = new ModalWindowEntity(text, okFun, blackCoverState.closeBlackCover);

      blackCoverState.setModalWindow(modalWindowEntity);
    };

    return (
      <div
        className={styles.chatInfoBurgerMenu}
        style={{ left: `${xMousePosition + 5}px`, top: `${yMousePosition - 5}px` }}
        onMouseLeave={() => setShowMenu(false)}
      >
        <button
          className={styles.changeChatDataButton}
          onClick={() => setUpdateMode(true)}
        >
          <EditSvg width={20} />
          <p>Edit</p>
        </button>
        <button className={styles.deleteChatButton} onClick={onClickDeleteChatHandler}>
          <TrashBinSvg width={20} />
          <p>Delete</p>
        </button>
      </div>
    );
  }
);

export default ChatInfoBurgerMenu;
