import React, { useState } from "react";
import { motion } from "framer-motion";
import { observer } from "mobx-react-lite";
import { useNavigate } from "react-router-dom";
import RouteConstants from "../../constants/RouteConstants";
import { RoleColor } from "../../models/enum/RoleColor";
import IUserDto from "../../models/interfaces/IUserDto";
import { authorizationState } from "../../state/AuthorizationState";
import { currentChatState } from "../../state/CurrentChatState";
import { currentProfileState } from "../../state/CurrentProfileState";
import nonAvatar from "../../assets/images/non_avatar.jpg";
import styles from "./MemberListItem.module.scss";
import MemberListItemBurgerMenu from "../memberListItemBurgerMenu/MemberListItemBurgerMenu";

const MemberListItem = observer((props: IUserDto) => {
  const [xMousePosition, setXMousePosition] = useState<number>(0);
  const [yMousePosition, setYMousePosition] = useState<number>(0);

  const [showMenu, setShowMenu] = useState<boolean>(false);

  var navigate = useNavigate();

  const currentChatStateChat = currentChatState.chat;
  const memberListItem = currentChatState.chat?.members.find(x => x.id === props.id);
  const areYouOwner = currentChatState.chat?.isOwner ?? false;
  const youRole = currentChatState.chat?.usersWithRole.find(x => x.userId === authorizationState.data?.id) ?? null;

  const isLastItem = (item: IUserDto | undefined) => {
    var array = currentChatState.chat?.members;

    if (!array || array.length === 0) {
      return false;
    }
    
    return item === array[array.length - 1];
  }

  const MouseMoveHandler = (event: React.MouseEvent<HTMLDivElement>) => {
    const targetCoords = event.currentTarget.getBoundingClientRect();
    const localX = event.clientX - targetCoords.left;
    const localY = event.clientY - targetCoords.top;

    setXMousePosition(localX);
    setYMousePosition(localY);
  };

  const showMenuHandler = (event: React.MouseEvent<HTMLDivElement>) => {
    event.preventDefault();
    setShowMenu(true);
  };

  const getMemberItemById = (userId: string) => {
    var role = currentChatStateChat?.usersWithRole.find(x => x.userId === userId);

    if (!role) return styles.memberItem;

    switch (role.roleColor) {
      case RoleColor.Green: return styles.memberItemGreenBackground
      case RoleColor.Blue: return styles.memberItemBlueBackground
      case RoleColor.Cyan: return styles.memberItemCyanBackground
      case RoleColor.Red: return styles.memberItemRedBackground
      case RoleColor.Yellow: return styles.memberItemYellowBackground
      case RoleColor.Orange: return styles.memberItemOrangeBackground
    }
  }

  const showProfileByMessage = async (userId: string) => {
    if (userId === authorizationState.data?.id) {
      currentProfileState.setProfileNull();

      return navigate(RouteConstants.Layout, { replace: true });
    }

    await currentProfileState
      .getUserAsync(userId)
      .catch((error: any) => { if (error.response.status !== 401) alert(error.response.data.message); });

    return navigate(RouteConstants.Layout, { replace: true });
  };

  return <motion.div
    initial={{ opacity: 0.7 }}
    animate={{ opacity: 1 }}
    transition={{ type: "Inertia", duration: .15 }}
    className={getMemberItemById(props.id)} key={props.id}
    onClick={() => showProfileByMessage(props.id)}
    onMouseMove={MouseMoveHandler}
    onContextMenu={showMenuHandler}>
    <img
      className={styles.memberItemAvatar}
      src={props.avatarLink ?? nonAvatar}
      alt=""
    />
    {
      showMenu && (
        <MemberListItemBurgerMenu
          x={xMousePosition}
          y={yMousePosition}
          isMemberListItemMine={props.id === authorizationState.data?.id}
          isMemberListItemLast={isLastItem(memberListItem)}
          yourRole={youRole}
          areYouOwner={areYouOwner}
          setShowMenu={setShowMenu} />
      )
    }
    <div className={styles.memberItemContainer}>
      <p className={styles.memberItemDisplayName}>{props.displayName}</p>
      <p className={styles.memberItemBio}>{props.bio}</p>
      <p className={styles.memberItemRole}>
        {
          currentChatStateChat?.ownerId == props.id && "owner" ||
          currentChatState.chat?.usersWithRole.find(
            (u) => u.userId === props.id
          )?.roleTitle
        }
      </p>
    </div>
  </motion.div>;
});

export default MemberListItem;