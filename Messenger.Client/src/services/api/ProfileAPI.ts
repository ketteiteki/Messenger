import IUserDto from "../../models/interfaces/IUserDto";
import api from "./baseAPI";

export default class ProfileApi {
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

    return await api.put<IUserDto>(`/Profile/update`, data);
  }

  public static async putUpdateProfileAvatarAsync(avatar: File) {
    const data = {
      avatar,
    };

    return await api.put<IUserDto>(`/Profile/updateAvatar`, data, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  }

  public static async delDeleteProfileAsync() {
    return await api.delete<IUserDto>(`/Profile/deleteProfile`);
  }
}
