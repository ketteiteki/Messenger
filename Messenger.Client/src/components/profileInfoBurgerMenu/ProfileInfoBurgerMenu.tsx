import React, { useState } from "react";
import { observer } from "mobx-react-lite";
import styles from "./ProfileInfoBurgerMenu.module.scss";
import { ReactComponent as EditSvg } from "../../assets/svg/edit_black.svg";
import { ReactComponent as TrashBinSvg } from "../../assets/svg/trash_bin_black.svg";
import { currentProfileState } from "../../state/CurrentProfileState";
import { authorizationState } from "../../state/AuthorizationState";
import { chatListWithMessagesState } from "../../state/ChatListWithMessagesState";
import { currentChatState } from "../../state/CurrentChatState";

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
    const currentProfileId = currentProfileState.date?.id;

    const deleteDialogHandler = async () => {
      if (authorizationId === undefined || currentChatState === undefined)
        return;

      const chatId = chatListWithMessagesState.data.find(
        (x) =>
          x.chat.members.find((m) => m.id !== authorizationId)?.id ===
          currentProfileState.date?.id
      )?.chat.id;

      if (chatId !== undefined) {
        chatListWithMessagesState.delDeleteDialogAsync(chatId, true);

        currentProfileState.setProfileNull();
        currentChatState.setChatAndMessagesNull();
      }
    };

    return (
      <div
        className={styles.profileInfoBurgerMenu}
        style={{ left: `${xMousePosition}px`, top: `${yMousePosition}px` }}
        onMouseLeave={() => setShowMenu(false)}
      >
        {currentProfileState.date?.id === authorizationState.data?.id ||
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
              onClick={currentProfileState.delDeleteProfileAsync}
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
        )}
      </div>
    );
  }
);

export default ProfileInfoBurgerMenu;
