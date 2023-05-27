import { makeAutoObservable, runInAction } from "mobx";
import ISessionDto from "../models/interfaces/ISessionDto";
import AuthorizationApi from "../services/api/AuthorizationApi";

class SessionsState {
  data: ISessionDto[] = [];

  constructor() {
    makeAutoObservable(
      this,
      {},
      {
        deep: true,
      }
    );
  }

  public clearData() {
    this.data = [];
  };

  //api
  public getSessionListAsync = async () => {
    const response = await AuthorizationApi.getSessionListAsync();

    if (response.status === 200) {
      this.data = response.data;
    }

    return response;
  };

  public delDeleteSessionAsync = async (sessionId: string) => {
    const response = await AuthorizationApi.delDeleteSessionAsync(sessionId);

    if (response.status === 200) {
      runInAction(() => {
        this.data = this.data.filter((i) => i.id !== sessionId);
      });
    }

    return response;
  };
}

export const sessionsState = new SessionsState();
