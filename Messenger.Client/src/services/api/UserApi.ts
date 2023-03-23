import IUserDto from "../../models/interfaces/IUserDto";
import api from "./baseApi";


export default class UsersApi {
    public static async getUserListBySearchAsync(search: string, limit: number, page: number) {
        const params = {
            search,
            limit,
            page
        };
    
        return await api.get<IUserDto[]>(`/Users/search`, {params});
    }

    public static async getUserListByChatAsync(chatId: string, limit: number, page: number) {
        const params = {
            limit,
            page
        };
    
        return await api.get<IUserDto[]>(`/Users/chat/${chatId}`, {params});
    }

    public static async getUserAsync(userId: string) {
        return await api.get<IUserDto>(`/Users/${userId}`);
    }
}