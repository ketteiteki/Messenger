

export default interface IAuthorizationResponse {
    id: string;
    displayName: string;
    nickname: string;
    bio: string | null;
    avatarLink: string | null;
    currentSessionId: string;
}