import CreateOrUpdateRoleUserEntity from "../../models/entities/CreateOrUpdateRoleUserEntity";
import CreatePermissionsUserInConversationEntity from "../../models/entities/CreatePermissionsUserInConversationEntity";
import IPermissionDto from "../../models/interfaces/IPermissionDto";
import IRoleUserByChatDto from "../../models/interfaces/IRoleUserByChatDto";
import IUserDto from "../../models/interfaces/IUserDto";
import api from "./baseAPI";



export default class ConversationApi {
    public static async getUserPermissionAsync(chatId: string, userId: string) {
        var params = {
            chatId,
            userId
        }

        return await api.get<IPermissionDto>(`/Conversations/getUserPermission`, { params });
    }

    public static async postConversationCreateOrUpdateRoleUserAsync(entity: CreateOrUpdateRoleUserEntity) {
        return await api.post<IRoleUserByChatDto>(`/Conversations/createOrUpdateRoleUser`, entity, {});
    }

    public static async postConversationCreatePermissionsAsync(entity: CreatePermissionsUserInConversationEntity) {
        return await api.post<IPermissionDto>(`/Conversations/createPermissions`, entity, {});
    }

    public static async postConversationAddUserAsync(chatId: string, userId: string) {
        const params = {
            chatId,
            userId
        };

        return await api.post<IUserDto>(`/Conversations/addUser`, null, { params });
    }

    public static async postConversationBanUserAsync(chatId: string, userId: string, banMinutes: number) {
        const params = {
            chatId,
            userId,
            banMinutes
        };

        return await api.post<IUserDto>(`/Conversations/banUser`, null, { params });
    }

    public static async postConversationUnbanUserAsync(chatId: string, userId: string) {
        const params = {
            chatId,
            userId,
        };

        return await api.post<IUserDto>(`/Conversations/unbanUser`, null, { params });
    }

    public static async postConversationRemoveUserAsync(chatId: string, userId: string) {
        const params = {
            chatId,
            userId,
        };

        return await api.post<IUserDto>(`/Conversations/removeUser`, null, { params });
    }

    public static async delConversationRemoveRoleUser(chatId: string, userId: string) {
        const params = {
            chatId,
            userId,
        };

        return await api.delete<IRoleUserByChatDto>(`/Conversations/removeRoleUser`, { params });
    }
}