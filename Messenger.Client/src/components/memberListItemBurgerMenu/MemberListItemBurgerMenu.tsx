import React, { MouseEvent, useState } from "react";
import { observer } from "mobx-react-lite";
import styles from "./MemberListItemBurgerMenu.module.scss";
import IRoleUserByChatDto from "../../models/interfaces/IRoleUserByChatDto";
import { currentChatState } from "../../state/CurrentChatState";

interface IMemberListItemBurgerMenuProps {
  x: number;
  y: number;
  isMemberListItemMine: boolean;
  areYouOwner: boolean;
  yourRole: IRoleUserByChatDto | null;
  isMemberListItemLast: boolean;
  memberListUserId: string;
  setShowMenu: React.Dispatch<React.SetStateAction<boolean>>;
}

const MemberListItemBurgerMenu = observer((
  { x,
    y,
    isMemberListItemMine,
    areYouOwner,
    yourRole,
    isMemberListItemLast,
    memberListUserId,
    setShowMenu }: IMemberListItemBurgerMenuProps) => {
  const [xMousePosition] = useState<number>(x - 7);
  const [yMousePosition] = useState<number>(y - 7);

  const banUser = async (event: MouseEvent<HTMLButtonElement>) => {
    event.stopPropagation();
    await currentChatState.banUserAsync(memberListUserId, 20);
  }

  const kickUser = async (event: MouseEvent<HTMLButtonElement>) => {
    event.stopPropagation();
    await currentChatState.kickUserAsync(memberListUserId);
  }

  return (
    <>
      {
        !isMemberListItemMine && <div className={styles.memberListItemBurgerMenu}
          style={{
            left: `${xMousePosition}px`,
            top: isMemberListItemLast ? `${yMousePosition - 40}px` : `${yMousePosition}px`
          }}
          onMouseLeave={() => setShowMenu(false)}>
          {
            areYouOwner &&
            <>
              <button className={styles.updateMessageButton}>
                <p>Change Permissions</p>
              </button>
              <button className={styles.updateMessageButton}>
                <p>Create Role For User</p>
              </button>
              <button className={styles.updateMessageButton} onClick={banUser}>
                <p>Ban User</p>
              </button>
              <button className={styles.updateMessageButton} onClick={kickUser}>
                <p>Kick User</p>
              </button>
            </>
          }
          {
            !areYouOwner && yourRole &&
            <>
              {
                yourRole.canGivePermissionToUser &&
                <button className={styles.updateMessageButton}>
                  <p>Change Permissions</p>
                </button>
              }
              {
                yourRole.canBanUser &&
                <button className={styles.updateMessageButton} onClick={banUser}>
                  <p>Ban User</p>
                </button>
              }
              {
                yourRole.canAddAndRemoveUserToConversation &&
                <button className={styles.updateMessageButton} onClick={kickUser}>
                  <p>Kick User</p>
                </button>
              }
            </>
          }
        </div>
      }
    </>
  );
});

export default MemberListItemBurgerMenu;