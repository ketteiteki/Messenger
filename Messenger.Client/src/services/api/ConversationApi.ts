import IUserDto from "../../models/interfaces/IUserDto";
import api from "./baseAPI";



export default class ConversationApi {
    public static async postConversationAddUserAsync(chatId: string, userId: string) {

        const params = {
            chatId,
            userId
        };
    
        return await api.post<IUserDto>(`/Conversations/addUser`, null, {params});
    }

    public static async postConversationBanUserAsync(chatId: string, userId: string, banMinutes: number) {

        const params = {
            chatId,
            userId,
            banMinutes
        };
    
        return await api.post<IUserDto>(`/Conversations/banUser`, null, {params});
    }

    public static async postConversationUnbanUserAsync(chatId: string, userId: string) {

        const params = {
            chatId,
            userId,
        };
    
        return await api.post<IUserDto>(`/Conversations/unbanUser`, null, {params});
    }

    public static async postConversationRemoveUserAsync(chatId: string, userId: string) {

        const params = {
            chatId,
            userId,
        };
    
        return await api.post<IUserDto>(`/Conversations/removeUser`, null, {params});
    }
}