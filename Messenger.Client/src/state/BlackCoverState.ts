import { makeAutoObservable } from "mobx";
import ModalWindowEntity from "../models/entities/ModalWindowEntity";

class BlackCoverState {
  public isBlackCoverShown: boolean = false;
  public imageLink: string | null = null;
  public modalWindow: ModalWindowEntity | null = null;

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

  public setModalWindow = (modalWindowEntity: ModalWindowEntity) => {
    this.isBlackCoverShown = true;
    this.modalWindow = modalWindowEntity;
  }

  public closeBlackCover = () => {
    this.isBlackCoverShown = false;
    this.imageLink = null;
    this.modalWindow = null;
  }
}

export const blackCoverState = new BlackCoverState();
