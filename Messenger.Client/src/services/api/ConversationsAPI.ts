import { IUser } from '../../models/interfaces/IUser';
import api from './baseAPI';


export default class ConversationAPI {
    public static async postConversationAddUserAsync(chatId: string, userId: string) {

        const params = {
            chatId,
            userId
        };
    
        return await api.post<IUser>(`/Conversations/addUser`, null, {params});
    }

    public static async postConversationBanUserAsync(chatId: string, userId: string, banMinutes: number) {

        const params = {
            chatId,
            userId,
            banMinutes
        };
    
        return await api.post<IUser>(`/Conversations/banUser`, null, {params});
    }

    public static async postConversationUnbanUserAsync(chatId: string, userId: string) {

        const params = {
            chatId,
            userId,
        };
    
        return await api.post<IUser>(`/Conversations/unbanUser`, null, {params});
    }

    public static async postConversationRemoveUserAsync(chatId: string, userId: string) {

        const params = {
            chatId,
            userId,
        };
    
        return await api.post<IUser>(`/Conversations/removeUser`, null, {params});
    }
}
