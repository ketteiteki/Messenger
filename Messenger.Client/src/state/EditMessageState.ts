import { makeAutoObservable } from 'mobx';
import EditMessageEntity from "../models/entities/EditMessageEntity";


class EditMessageState {
  public data: EditMessageEntity | null = null;

  constructor() {
    makeAutoObservable(
      this,
      {},
      {
        deep: true,
      }
    );
  }

  public setEditMessage = (editMessageEntity: EditMessageEntity) => {
    this.data = editMessageEntity;
  }

  public setEditMessageNull = () => {
    this.data = null;
  }
} 

export const editMessageState = new EditMessageState();