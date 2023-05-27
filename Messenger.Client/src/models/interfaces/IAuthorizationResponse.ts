

export default interface IAuthorizationResponse {
    accessToken: string;
    refreshToken: string;
    id: string;
    displayName: string;
    nickname: string;
    bio: string | null;
    avatarLink: string | null;
    currentSessionId: string;
}