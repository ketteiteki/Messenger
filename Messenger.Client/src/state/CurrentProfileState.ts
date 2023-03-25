import { makeAutoObservable, runInAction } from "mobx";
import IUserDto from "../models/interfaces/IUserDto";
import ProfileApi from "../services/api/ProfileAPI";
import UsersApi from "../services/api/UserApi";

class CurrentProfileState {
  date: IUserDto | null = null;

  constructor() {
    makeAutoObservable(
      this,
      {},
      {
        deep: true,
      }
    );
  }

  public setProfileNull() {
    this.date = null;
  }

  //api
  public getUserAsync = async (userId: string) => {
    const response = await UsersApi.getUserAsync(userId);

    if (response.status === 200) {
      runInAction(() => {
        this.date = response.data;
      });
    }

    return response;
  };

  public delDeleteProfileAsync = async () => {
    const response = await ProfileApi.delDeleteProfileAsync()
  }
}

export const currentProfileState = new CurrentProfileState();
