import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { useAppDispatch, useAppSelector } from "../app/hooks";
import {
  clearAuthorizationData,
  selectAuthorization,
} from "../app/slices/authorizationSlice";
import {
  selectCurrentProfile,
  setNullCurrentProfile,
} from "../app/slices/currentProfileSlice";
import { ReactComponent as TickSVG } from "../assets/svg/tick.svg";
import { ReactComponent as RubbishBinSVG } from "../assets/svg/rubbish-bin.svg";
import { ReactComponent as CloseCrossSVG } from "../assets/svg/close-cross.svg";
import { putUpdateProfileAsync } from "../app/thunks/profileThuck";
import ProfileAPI from "../services/api/ProfileAPI";
import TokenService from "../services/messenger/TokenService";
import {
  removeAllSessionLocal,
  selectSession,
} from "../app/slices/sessionSlice";
import {
  delDeleteSessionAsync,
  getSessionListAsync,
} from "../app/thunks/authorizationThucks";
import DateService from "../services/messenger/DateService";

export const Profile = () => {
  const navigate = useNavigate();

  const authorizationState = useAppSelector(selectAuthorization);
  const currentProfileState = useAppSelector(selectCurrentProfile);
  const sessionState = useAppSelector(selectSession);
  const dispatch = useAppDispatch();

  const [displayName, setDisplayName] = useState<string>(
    currentProfileState.data
      ? currentProfileState.data?.displayName || ""
      : authorizationState.data?.displayName || ""
  );
  const [nickName, setNickname] = useState<string>(
    currentProfileState.data
      ? currentProfileState.data?.nickname || ""
      : authorizationState.data?.nickName || ""
  );
  const [bio, setBio] = useState<string>(
    currentProfileState.data
      ? currentProfileState.data?.bio || ""
      : authorizationState.data?.bio || ""
  );

  const avatarProfile = currentProfileState.data
    ? currentProfileState.data?.avatarLink ||
      "https://i.pinimg.com/564x/d9/eb/32/d9eb32e507d11b4e5de6bcab602d3c66.jpg"
    : authorizationState.data?.avatarLink ||
      "https://i.pinimg.com/564x/d9/eb/32/d9eb32e507d11b4e5de6bcab602d3c66.jpg";

  const isMe = currentProfileState.data === null;

  const onClickChangeProfileData = () => {
    dispatch(putUpdateProfileAsync({ displayName, nickName, bio }));
  };

  const onClickDeleteProfile = async () => {
    const result = await ProfileAPI.delDeleteProfileAsync();

    if (result.status == 200) {
      dispatch(clearAuthorizationData());
      TokenService.deleteLocalAccessToken();
      TokenService.deleteLocalRefreshToken();
      return navigate("/registration", { replace: true });
    }
  };

  const onClickDeleteSession = (sessionId: string) => {
    dispatch(delDeleteSessionAsync(sessionId));
  };

  useEffect(() => {
    dispatch(getSessionListAsync());

    return () => {
      dispatch(setNullCurrentProfile());
      dispatch(removeAllSessionLocal());
    };
  }, []);

  return (
    <div className="profile">
      {(authorizationState.data?.displayName !== displayName ||
        authorizationState.data?.nickName !== nickName ||
        authorizationState.data?.bio !== bio) &&
        isMe && (
          <button
            className="profile__comfirm-change-data"
            onClick={onClickChangeProfileData}
          >
            <TickSVG />
          </button>
        )}
      {isMe && (
        <RubbishBinSVG
          className="profile__rubbish-bin"
          onClick={onClickDeleteProfile}
        />
      )}
      <img className="profile__avatar" src={avatarProfile} alt="" />
      <input
        className="profile__display-name"
        disabled={!isMe}
        value={displayName}
        onChange={(e) => setDisplayName(e.currentTarget.value)}
      />
      <input
        className="profile__nickname"
        disabled={!isMe}
        value={nickName}
        onChange={(e) => setNickname(e.currentTarget.value)}
      />
      <input
        className="profile__bio"
        disabled={!isMe}
        value={bio}
        onChange={(e) => setBio(e.currentTarget.value)}
      />
      {isMe || <button className="profile__start-dialog">Start dialog</button>}
      {isMe && (
        <div className="profile__session-list">
          {sessionState.data.map((s) => (
            <div className="profile__session-list__session-item" key={s.id}>
              <CloseCrossSVG
                className="profile__session-list__session-item__close-cross"
                onClick={() => onClickDeleteSession(s.id)}
              />
              <p>Ip: {s.ip}</p>
              <p>User-Agent: {s.userAgent}</p>
              <p>CreateAt: {DateService.getDateAndTime(s.createAt)}</p>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};
