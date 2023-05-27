import React, { useState } from "react";
import { observer } from "mobx-react-lite";
import styles from "./ProfileInfoBurgerMenu.module.scss";
import { ReactComponent as EditSvg } from "../../assets/svg/edit_black.svg";
import { ReactComponent as TrashBinSvg } from "../../assets/svg/trash_bin_black.svg";
import { ReactComponent as LoguoutSvg } from "../../assets/svg/logout.svg";
import { currentProfileState } from "../../state/CurrentProfileState";
import { authorizationState } from "../../state/AuthorizationState";
import { chatListWithMessagesState } from "../../state/ChatListWithMessagesState";
import { currentChatState } from "../../state/CurrentChatState";
import { blackCoverState } from "../../state/BlackCoverState";
import ModalWindowEntity from "../../models/entities/ModalWindowEntity";
import { useNavigate } from "react-router-dom";
import TokenService from "../../services/messenger/TokenService";

interface ProfileInfoBurgerMenu {
  x: number;
  y: number;
  profileId: string;
  setShowMenu: React.Dispatch<React.SetStateAction<boolean>>;
  setUpdateMode: React.Dispatch<React.SetStateAction<boolean>>;
}

const ProfileInfoBurgerMenu = observer(
  ({ x, y, setShowMenu, setUpdateMode }: ProfileInfoBurgerMenu) => {
    const [xMousePosition, setXMousePosition] = useState<number>(x - 100);
    const [yMousePosition, setYMousePosition] = useState<number>(y);

    const authorizationId = authorizationState.data?.id;

    const navigate = useNavigate();

    const onClickLogoutHandler = () => {

      const text = "Do you want to log out?";

      const okFun = async () => {
        TokenService.deleteLocalAccessToken();
        TokenService.deleteLocalRefreshToken();
        navigate("/login", { replace: true });
      };

      const modalWindowEntity = new ModalWindowEntity(text, okFun, blackCoverState.closeBlackCover);

      blackCoverState.setModalWindow(modalWindowEntity);
    };

    const onClickDeleteProfileHandler = async () => {
      const text = "Do you want to delete your account?";

      const okFun = async () => {
        try {
          await currentProfileState.delDeleteProfileAsync();
          navigate("/registration", { replace: true });
        } catch (error: any) {
          alert(error.response.data.message);
        }
      };

      const modalWindowEntity = new ModalWindowEntity(text, okFun, blackCoverState.closeBlackCover);

      blackCoverState.setModalWindow(modalWindowEntity);
    };

    const deleteDialogHandler = async () => {
      const text = "Do you want to delete the dialog?";

      const okFun = async () => {
        if (authorizationId === undefined || currentChatState === undefined)
          return;

        const chatId = chatListWithMessagesState.data.find((x) =>
          x.chat.members.find((m) => m.id !== authorizationId)?.id ===
          currentProfileState.date?.id
        )?.chat.id;

        if (chatId !== undefined) {
          try {
            await chatListWithMessagesState.delDeleteDialogAsync(chatId, true);
          } catch (error: any) {
            alert(error.response.data.message);
          }

          currentProfileState.setProfileNull();
          currentChatState.setChatAndMessagesNull();
        }
      };

      const modalWindowEntity = new ModalWindowEntity(text, okFun, blackCoverState.closeBlackCover);

      blackCoverState.setModalWindow(modalWindowEntity);
    };

    return (
      <div
        className={styles.profileInfoBurgerMenu}
        style={{ left: `${xMousePosition + 5}px`, top: `${yMousePosition - 5}px` }}
        onMouseLeave={() => setShowMenu(false)}
      >
        {
          currentProfileState.date?.id === authorizationState.data?.id ||
            currentProfileState.date === null ? (
            <>
              <button
                className={styles.editAccountButton}
                onClick={() => setUpdateMode(true)}
              >
                <EditSvg width={20} />
                <p>Edit</p>
              </button>
              <button
                className={styles.deleteButton}
                onClick={onClickLogoutHandler}
              >
                <LoguoutSvg width={20} />
                <p>Logout</p>
              </button>
              <button
                className={styles.deleteButton}
                onClick={onClickDeleteProfileHandler}
              >
                <TrashBinSvg width={20} />
                <p>Delete</p>
              </button>
            </>
          ) : (
            <>
              <button
                className={styles.deleteButton}
                onClick={deleteDialogHandler}
              >
                <TrashBinSvg width={20} />
                <p>Delete</p>
              </button>
            </>
          )
        }
      </div>
    );
  }
);

export default ProfileInfoBurgerMenu;
