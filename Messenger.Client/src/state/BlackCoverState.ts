import { action, makeAutoObservable, runInAction } from "mobx";

class BlackCoverState {
  public isBlackCoverShown: boolean = false;
  public imageLink: string | null = null;

  constructor() {
    makeAutoObservable(
      this,
      {},
      {
        deep: true,
      }
    );
  }

  public setImage = (imageLink: string) => {
    this.isBlackCoverShown = true;
    this.imageLink = imageLink;
  }

  public closeBlackCover = () => {
    this.isBlackCoverShown = false;
    this.imageLink = null;
  }
}

export const blackCoverState = new BlackCoverState();
