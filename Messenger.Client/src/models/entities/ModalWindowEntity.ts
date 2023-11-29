

export default class ModalWindowEntity {
  public text: string;
  public okAction: () => void;
  public cancelAction: () => void;

  constructor(text: string, okAction: () => void,  cancelAction: () => void) {
    this.text = text;
    this.okAction = okAction;
    this.cancelAction = cancelAction;
  }
}