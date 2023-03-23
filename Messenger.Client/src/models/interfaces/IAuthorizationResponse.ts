

export default interface IAuthorizationResponse {
    accessToken: string;
    refreshToken: string;
    id: string;
    displayName: string;
    nickName: string;
    bio: string | null;
    avatarLink: string | null;
}