import IAuthorizationResponse from "../../models/interfaces/IAuthorizationResponse";
import IUserSessionDto from "../../models/interfaces/IUserSessionDto";
import api from "./baseAPI";

export default class AuthorizationApi {
    public static async getAuthorizationAsync() {
        return await api.get<IAuthorizationResponse>(`/Auth/authorization`);
    }

    public static async getSessionListAsync() {
        return await api.get<IUserSessionDto[]>("/Auth/sessions");
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

    public static async postLogoutAsync() {
        return await api.post("/Auth/logout");
    }

    public static async delTerminateSessionAsync(sessionId: string) {
        return await api.delete<IUserSessionDto>(`/Auth/sessions/${sessionId}`, );
    }
}