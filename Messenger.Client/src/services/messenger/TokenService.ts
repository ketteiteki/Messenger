


export default class TokenService {
    public static getLocalAccessToken(): string | null {
        return localStorage.getItem("AuthorizationToken");
    }

    public static setLocalAccessToken(value: string): void {
        return localStorage.setItem("AuthorizationToken", value);
    }

    public static getLocalRefreshToken(): string | null {
        return localStorage.getItem("RefreshToken");
    }

    public static setLocalRefreshToken(value: string): void {
        return localStorage.setItem("RefreshToken", value);
    }

    public static deleteLocalAccessToken(): void {
        return localStorage.removeItem("AuthorizationToken");
    }

    public static deleteLocalRefreshToken(): void {
        return localStorage.removeItem("RefreshToken");
    }
}