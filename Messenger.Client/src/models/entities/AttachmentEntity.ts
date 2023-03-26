import IAttachmentDto from "../interfaces/IAttachmentDto";


export default class AttachmentEntity implements IAttachmentDto {
  id: string;
  size: number;
  link: string;

  constructor(id: string, size: number, link: string) {
    this.id = id;
    this.size = size;
    this.link = link;
  }
}