import { IUser } from "../../models/interfaces/IUser";
import api from "./baseAPI";

export default class ProfileAPI {
  public static async putUpdateProfileAsync(
    displayName: string,
    nickName: string,
    bio: string
  ) {
    const data = {
      displayName,
      nickName,
      bio,
    };

    return await api.put<IUser>(`/Profile/update`, data);
  }

  public static async putUpdateProfileAvatarAsync(avatarFile: File) {
    const formData = new FormData();

    formData.append("Avatar", avatarFile);

    return await api.put<IUser>(`/Profile/updateAvatar`, formData);
  }

  public static async delDeleteProfileAsync() {
    return await api.delete<IUser>(`/Profile/deleteProfile`);
  }
}