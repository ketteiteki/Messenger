import { makeAutoObservable, runInAction } from "mobx";
import IAuthorizationResponse from "../models/interfaces/IAuthorizationResponse";
import AuthorizationApi from "../services/api/AuthorizationApi";
import ProfileApi from "../services/api/ProfileAPI";

class AuthorizationState {
  public data: IAuthorizationResponse | null = null;

  constructor() {
    makeAutoObservable(
      this,
      {},
      {
        deep: true,
      }
    );
  }

  public clearAuthorizationData = () => {
    this.data = null;
  };

  //api
  public getAuthorizationAsync = async () => {
    const response = await AuthorizationApi.getAuthorizationAsync();

    if (response.status === 200) {
      runInAction(() => {
        this.data = response.data;
      });
    }

    return response;
  };

  public postRegistrationAsync = async (
    displayName: string,
    nickname: string,
    password: string
  ) => {
    const response = await AuthorizationApi.postRegistrationAsync(
      displayName,
      nickname,
      password
    );

    if (response.status === 200) {
      runInAction(() => {
        this.data = response.data;
      });
    }

    return response;
  };

  public postLoginAsync = async (nickname: string, password: string) => {
    const response = await AuthorizationApi.postLoginAsync(nickname, password);

    if (response.status === 200) {
      runInAction(() => {
        this.data = response.data;
      });
    }

    return response;
  };

  public putUpdateProfileAsync = async (
    displayName: string,
    nickname: string,
    bio: string
  ) => {
    const response = await ProfileApi.putUpdateProfileAsync(
      displayName,
      nickname,
      bio
    );

    if (response.status === 200) {
      runInAction(() => {
        if (this.data) {
          this.data.displayName = response.data.displayName;
          this.data.nickname = response.data.nickname;
          this.data.bio = response.data.bio;
        }
      });
    }

    return response;
  };

  public putUpdateProfileAvatarAsync = async (avatarFile: File) => {
    const response = await ProfileApi.putUpdateProfileAvatarAsync(avatarFile);

    if (response.status === 200) {
      runInAction(() => {
        if (this.data) {
          this.data.avatarLink = response.data.avatarLink;
        }
      });
    }

    return response;
  };
}

export const authorizationState = new AuthorizationState();
