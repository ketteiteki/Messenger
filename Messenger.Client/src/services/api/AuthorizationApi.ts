import IAuthorizationResponse from "../../models/interfaces/IAuthorizationResponse";
import ISessionDto from "../../models/interfaces/ISessionDto";
import api from "./baseAPI";

export default class AuthorizationApi {
    public static async getAuthorizationAsync(accessToken: string) {
        return await api.get<IAuthorizationResponse>(`/Auth/authorization`);
    }

    public static async getSessionAsync(accessToken: string) {
        return await api.get<ISessionDto>(`/Auth/sessions/${accessToken}`);
    }

    public static async getSessionListAsync() {
        return await api.get<ISessionDto[]>("/Auth/sessions");
    }

    public static async postRegistrationAsync(displayName: string, nickname: string, password: string) {
        const data = {
            displayName,
            nickname,
            password
        }
    
        return await api.post<IAuthorizationResponse>("/Auth/registration", data);
    }

    public static async postLoginAsync(nickname: string, password: string) {
        const data = {
            nickname,
            password
        }
    
        return await api.post<IAuthorizationResponse>("/Auth/login", data);
    }

    public static async delDeleteSessionAsync(sessionId: string) {
        return await api.delete<ISessionDto>(`/Auth/sessions/${sessionId}`, );
    }
}