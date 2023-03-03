import { makeAutoObservable } from "mobx";
import ReplyEntity from "../models/entities/ReplyEntity";

class ReplyState {
  public data: ReplyEntity | null = null;

  constructor() {
    makeAutoObservable(
      this,
      {},
      {
        deep: true,
      }
    );
  }

  public setReply = (replyEntity: ReplyEntity) => {
    this.data = replyEntity;
  }

  public setReplyNull = () => {
    this.data = null;
  }
}

export const replyState = new ReplyState();
