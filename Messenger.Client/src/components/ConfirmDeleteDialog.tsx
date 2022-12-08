import React, { useState } from "react";
import { useAppDispatch, useAppSelector } from "../app/hooks";
import { selectCurrentChat, currentChatSliceActions } from "../app/slices/currentChatSlice";
import { setShowBlackBackground, setShowConfirmDeleteDialog } from "../app/slices/layoutComponentSlice";
import { delDeleteDialogAsync } from "../app/thunks/dialogsThuck";

interface IConfirmDeleteDialogProps {
  displayNameUser: string;
}

export const ConfirmDeleteDialog = ({
  displayNameUser,
}: IConfirmDeleteDialogProps) => {
  const currentChatState = useAppSelector(selectCurrentChat);
  const dispatch = useAppDispatch();

  const [deleteForAll, setDeleteForAll] = useState<boolean>(false);

  const onClickDeleteDialog = () => {
    if (!currentChatState.data) return;

    dispatch(delDeleteDialogAsync({dialogId: currentChatState.data.id, isDeleteForAll: deleteForAll}));
    dispatch(currentChatSliceActions.setCurrentChatData(null));

    dispatch(setShowBlackBackground(false));
    dispatch(setShowConfirmDeleteDialog(false));
  }

  return (
    <div className="confirm-delete-dialog">
      <div className="confirm-delete-dialog__checkbox-and-text">
        <input
          type="checkbox"
          className="confirm-delete-dialog__checkbox-and-text__checkbox"
          checked={deleteForAll}
          onClick={() => setDeleteForAll(!deleteForAll)}
        />
        <p className="confirm-delete-dialog__checkbox-and-text__text">
          Also delete for {displayNameUser}
        </p>
      </div>
      <button className="confirm-delete-dialog__delete-dialog-button" onClick={onClickDeleteDialog}>
        Delete
      </button>
    </div>
  );
};
