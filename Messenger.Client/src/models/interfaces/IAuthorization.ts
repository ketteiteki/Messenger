export interface IAuthorization {
  accessToken: string;
  refreshToken: string;
  id: string;
  displayName: string;
  nickName: string;
  bio: string | null;
  avatarLink: string | null;
}
