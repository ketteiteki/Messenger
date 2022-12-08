import { IAuthorization } from "../../models/interfaces/IAuthorization";
import { ISession } from "../../models/interfaces/ISession";
import api from "./baseAPI";

export default class AuthAPI {
    public static async getAuthorizationAsync(accessToken: string) {
        return await api.get<IAuthorization>(`/Auth/authorization/${accessToken}`);
    }

    public static async getSessionAsync(accessToken: string) {
        return await api.get<ISession>(`/Auth/sessions/${accessToken}`);
    }

    public static async getSessionListAsync() {
        return await api.get<ISession[]>("/Auth/sessions");
    }

    public static async postRegistrationAsync(displayName: string, nickname: string, password: string) {
        const data = {
            displayName,
            nickname,
            password
        }
    
        return await api.post<IAuthorization>("/Auth/registration", data);
    }

    public static async postLoginAsync(nickname: string, password: string) {
        const data = {
            nickname,
            password
        }
    
        return await api.post<IAuthorization>("/Auth/login", data);
    }

    public static async delDeleteSessionAsync(sessionId: string) {
        return await api.delete<ISession>(`/Auth/sessions/${sessionId}`, );
    }
}