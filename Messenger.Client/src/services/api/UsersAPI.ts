import { IUser } from './../../models/interfaces/IUser';
import api from "./baseAPI";

export default class UsersAPI {
    public static async getUserListBySearchAsync(search: string, limit: number, page: number) {
        const params = {
            search,
            limit,
            page
        };
    
        return await api.get<IUser[]>(`/Users/search`, {params});
    }

    public static async getUserListByChatAsync(chatId: string, limit: number, page: number) {
        const params = {
            limit,
            page
        };
    
        return await api.get<IUser[]>(`/Users/chat/${chatId}`, {params});
    }

    public static async getUserAsync(userId: string) {
        return await api.get<IUser>(`/Users/${userId}`);
    }
}