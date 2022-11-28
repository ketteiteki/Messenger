import React, { useState, useEffect } from "react";
import { Chat } from "../components/Chat";
import { ChatBar } from "../components/ChatBar";
import { ChatCreater } from "../components/ChatCreater";
import { Profile } from "../components/Profile";
import { ChatInfo } from "../components/ChatInfo";
import { useNavigate } from "react-router";
import { useAppDispatch, useAppSelector } from "../app/hooks";
import { selectAuthorization } from "../app/slices/authorizationSlice";
import { RequestStatusEnum } from "../models/enums/RequestStatusEnum";
import { authorizationAsync } from "../app/thunks/authorizationThucks";
import { getChatListAsync } from "../app/thunks/chatsThuck";
import { ConfirmDeleteDialog } from "../components/ConfirmDeleteDialog";
import { selectCurrentChat } from "../app/slices/currentChatSlice";
import {
  selectLayoutComponent,
  setShowBlackBackground,
  setShowChatCreater,
  setShowChatInfo,
  setShowConfirmDeleteDialog,
  setShowProfile,
} from "../app/slices/layoutComponentSlice";

export const Layout = () => {
  const authorizationState = useAppSelector(selectAuthorization);
  const layoutComponentState = useAppSelector(selectLayoutComponent);
  const currentChatState = useAppSelector(selectCurrentChat);
  const dispatch = useAppDispatch();
  
  const [loading, setLoading] = useState<boolean>(true);

  const navigate = useNavigate();

  const displayNameUserForConfirmDeleteChat =
    currentChatState.data?.members?.find(
      (m) => m.id != authorizationState.data?.id
    )?.displayName;

  useEffect(() => {
    const accessToken = localStorage.getItem("AuthorizationToken");

    const fun = async () => {
      if (accessToken == null) {
        return navigate("/registration", { replace: true });
      }

      const result = await dispatch(authorizationAsync({ accessToken }));

      if (!result.payload) {
        return navigate("/registration", { replace: true });
      }

      setLoading(false);

      if (!loading) {
        await dispatch(getChatListAsync());
      }
    };

    fun();
  }, []);

  const onClickBlackBackground = () => {
    dispatch(setShowBlackBackground(false));
    dispatch(setShowChatCreater(false));
    dispatch(setShowChatInfo(false));
    dispatch(setShowConfirmDeleteDialog(false));
    dispatch(setShowProfile(false));
  };

  return (
    <div className="layout">
      <div className="layout__background" />
      {layoutComponentState.data.showConfirmDeleteDialog && (
        <ConfirmDeleteDialog
          displayNameUser={displayNameUserForConfirmDeleteChat || ""}
        />
      )}
      {loading ? (
        <div className="layout__loading" />
      ) : (
        <>
          {layoutComponentState.data.showBlackBackground && (
            <div
              className="black-background"
              onClick={onClickBlackBackground}
            />
          )}

          <ChatBar />
          <Chat />
          {layoutComponentState.data.showProfile && <Profile />}
          {layoutComponentState.data.showChatCreater && <ChatCreater />}
          {layoutComponentState.data.showChatInfo && <ChatInfo />}
        </>
      )}
    </div>
  );
};
