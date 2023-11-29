import { makeAutoObservable, runInAction } from "mobx";
import { CreateChatEnum } from "../models/enum/CreateChat";
import IChatDto from "../models/interfaces/IChatDto";
import IMessageDeleteNotificationDto from "../models/interfaces/IMessageDeleteNotificationDto";
import IMessageDto from "../models/interfaces/IMessageDto";
import IMessageUpdateNotificationDto from "../models/interfaces/IMessageUpdateNotificationDto";
import ChatApi from "../services/api/ChatApi";
import DialogApi from "../services/api/DialogApi";
import MessagesApi from "../services/api/MessageApi";
import { IChatListWithMessagesDataItem } from "./types/ChatListWithMessagesStateTypes";

class ChatListWithMessagesState {
  public data: IChatListWithMessagesDataItem[] = [];
  public dataForSearchChats: IChatListWithMessagesDataItem[] = [];
  public searchInput: string = "";

  constructor() {
    makeAutoObservable(
      this,
      {},
      {
        deep: true,
      }
    );
  }

  public setSearchInput = (text: string) => {
    this.searchInput = text;
  }

  public setLastMessage = (message: IMessageDto) => {
    const item = this.data.find((c) => c.chat.id === message.chatId);

    if (!item) return;

    item.chat.lastMessageId = message.id;
    item.chat.lastMessageText = message.text;
    item.chat.lastMessageAuthorDisplayName = message.ownerDisplayName;
    item.chat.lastMessageDateOfCreate = message.dateOfCreate;
  };

  public addMessageInData = (message: IMessageDto) => {
    const item = this.data.find((c) => c.chat.id === message.chatId);
    const itemInSearchData = this.dataForSearchChats.find((c) => c.chat.id === message.chatId);

    item?.messages.unshift(message);
    itemInSearchData?.messages.unshift(message);
  };

  public updateMessageInData = (
    messageUpdateNotification: IMessageUpdateNotificationDto
  ) => {
    const item = this.data.find(
      (c) => c.chat.id === messageUpdateNotification.chatId
    );

    if (!item) return;

    const message = item.messages.find(
      (x) => x.id === messageUpdateNotification.messageId
    );

    if (!message) return;

    message.text = messageUpdateNotification.updatedText;
    message.isEdit = true;

    if (messageUpdateNotification.isLastMessage) {
      item.chat.lastMessageText = messageUpdateNotification.updatedText;
    }
  };

  public deleteMessageInDataByMessageDeleteNotification = (
    messageDeleteNotification: IMessageDeleteNotificationDto
  ) => {
    const dataItem = this.data.find((c) => c.chat.id === messageDeleteNotification.chatId);

    if (!dataItem) return;

    const messageIndex = dataItem.messages.findIndex((x) => x.id === messageDeleteNotification.messageId);

    if (messageIndex === -1) return;

    dataItem.messages.splice(messageIndex, 1);

    const lastMessageNow = dataItem?.messages[dataItem?.messages.length - 1];

    dataItem.chat.lastMessageId = lastMessageNow?.id ?? null;
    dataItem.chat.lastMessageAuthorDisplayName = lastMessageNow?.ownerDisplayName ?? null;
    dataItem.chat.lastMessageText = lastMessageNow?.text ?? null;
    dataItem.chat.lastMessageDateOfCreate = lastMessageNow?.dateOfCreate ?? null;
  };

  public addChatInData = (chat: IChatDto, messages: IMessageDto[]) => {
    this.data.unshift({ chat, messages });
  };

  public resetDataForSearchChats = () => {
    this.dataForSearchChats = [];
  };

  public deleteChatInDataById = (chatId: string) => {
    const item = this.data.findIndex((c) => c.chat.id === chatId);

    if (!item) return;

    this.data.splice(item, 1);
  };

  public pushChatOnTop = (chatId: string) => {
    const itemIndex = this.data.findIndex((x) => x.chat.id === chatId);

    if (itemIndex === -1) return;

    const chat = this.data[itemIndex];

    this.data.splice(itemIndex, 1);

    this.data.splice(0, 0, chat);
  };

  public clearChatListWithMessagesData = () => {
    this.data = [];
    this.dataForSearchChats = [];
    this.searchInput = "";
  };

  //api
  public getMessageListAsync = async (
    chatId: string,
    fromMessageDateTime: string | null
  ) => {
    const response = await MessagesApi.getMessageListAsync(
      chatId,
      fromMessageDateTime,
      30
    );

    if (response.status === 200) {
      runInAction(() => {
        const dataItem = this.data.find((i) => i.chat.id === chatId);
        const dataForSearchItem = this.dataForSearchChats.find(
          (i) => i.chat.id === chatId
        );

        const messages = response.data;

        dataItem?.messages.push(...messages);
        dataForSearchItem?.messages.push(...messages);
      });
    }

    return response;
  };

  public getChatListAsync = async () => {
    const response = await ChatApi.getChatListAsync();

    runInAction(() => {
      response.data.forEach((c) => this.data.push({ chat: c, messages: [] }));
    });

    return response;
  };

  public getChatListBySearchAsync = async (searchText: string) => {
    const response = await ChatApi.getChatListBySearchAsync(searchText);

    runInAction(() => {
      this.dataForSearchChats = [];
      response.data.forEach((c) =>
        this.dataForSearchChats.push({ chat: c, messages: [] })
      );
    });

    return response;
  };

  public putUpdateMessageAsync = async (
    chatId: string,
    messageId: string,
    text: string
  ) => {
    const chatItem = this.data.find((c) => c.chat.id === chatId);

    if (!chatItem) return;

    const message = chatItem.messages.find((x) => x.id === messageId);

    if (!message) return;
    message.text = text;
    message.loading = true;
    message.isEdit = true;

    const response = await MessagesApi.putUpdateMessageAsync(messageId, text);

    if (response.status === 200) {
      runInAction(() => {
        message.loading = false;

        if (
          chatItem.messages.length - 1 ===
          chatItem.messages.findIndex((x) => x.id === messageId)
        ) {
          chatItem.chat.lastMessageText = text;
        }
      });
    }

    return response;
  };

  public delDeleteMessageAsync = async (chatId: string, messageId: string) => {
    const dataItem = this.data.find((c) => c.chat.id === chatId);

    if (!dataItem) return;

    const messageIndex = dataItem.messages.findIndex((x) => x.id === messageId);

    if (messageIndex === -1) return;

    dataItem.messages.splice(messageIndex, 1);

    const lastMessageNow = dataItem?.messages[dataItem?.messages.length - 1];

    dataItem.chat.lastMessageId = lastMessageNow?.id ?? null;
    dataItem.chat.lastMessageAuthorDisplayName = lastMessageNow?.ownerDisplayName ?? null;
    dataItem.chat.lastMessageText = lastMessageNow?.text ?? null;
    dataItem.chat.lastMessageDateOfCreate = lastMessageNow?.dateOfCreate ?? null;

    const response = await MessagesApi.delDeleteMessageAsync(messageId, true);

    return response;
  };

  public postCreateDialogAsync = async (userId: string) => {
    const response = await DialogApi.postCreateDialogAsync(userId);

    if (response.status === 200) {
      runInAction(() => {
        this.data.push({ chat: response.data, messages: [] });
      });
    }

    return response;
  };

  public postCreateMessageAsync = async (
    messageEntity: IMessageDto,
    files: File[]
  ) => {
    const response = await MessagesApi.postCreateMessageAsync(
      messageEntity.text,
      messageEntity.chatId,
      messageEntity.replyToMessageId,
      files
    );

    if (response.status === 200) {
      runInAction(() => {
        const dataItem = this.data.find(x => x.chat.id === messageEntity.chatId);
        const messageItem = dataItem?.messages.find(x => x.id === messageEntity.id);

        if (!messageItem) return;

        messageItem.id = response.data.id;
        messageItem.loading = false;
        messageEntity.attachments = response.data.attachments;
        chatListWithMessagesState.setLastMessage(response.data);
      });
    }

    return response;
  };

  public postCreateChatAsync = async (
    name: string,
    title: string,
    type: CreateChatEnum,
    avatarFile: File | null
  ) => {
    const response = await ChatApi.postCreateChatAsync(
      name,
      title,
      type,
      avatarFile
    );

    runInAction(() => {
      this.data.unshift({ chat: response.data, messages: [] });
    });

    return response;
  };

  public putUpdateChatDataAsync = async (
    chatId: string,
    name: string,
    title: string
  ) => {
    const response = await ChatApi.putUpdateChatDataAsync(chatId, name, title);

    if (response.status === 200) {
      runInAction(() => {
        this.data.map((i) => {
          if (i.chat.id === response.data.id) {
            i.chat.title = response.data.title;
            i.chat.name = response.data.name;
            return { chat: i.chat, messages: i.messages };
          }
          return { chat: i.chat, messages: i.messages };
        });
      });
    }

    return response;
  };

  public postLeaveFromChatAsync = async (chatId: string) => {
    const response = await ChatApi.postLeaveFromChatAsync(chatId);

    if (response.status === 200) {
      runInAction(() => {
        this.data = this.data.filter((x) => x.chat.id !== chatId);
      });
    }

    return response;
  };

  public putUpdateChatAvatarAsync = async (chatId: string, avatarFile: File) => {
    const response = await ChatApi.putUpdateChatAvatarAsync(chatId, avatarFile);

    if (response.status === 200) {
      runInAction(() => {
        const item = this.data.find((c) => c.chat.id === chatId);

        if (!item) return;

        item.chat.avatarLink = response.data.avatarLink;
      });
    }
  };

  public delDeleteDialogAsync = async (
    dialogId: string,
    isDeleteForAll: boolean
  ) => {
    const response = await DialogApi.delDeleteDialogAsync(
      dialogId,
      isDeleteForAll
    );

    if (response.status === 200) {
      runInAction(() => {
        this.data = this.data.filter((x) => x.chat.id !== dialogId);
      });
    }

    return response;
  };

  public delDeleteChatAsync = async (chatId: string) => {
    const response = await ChatApi.delDeleteChatAsync(chatId);

    if (response.status === 200) {
      this.data = this.data.filter((x) => x.chat.id !== chatId);
    }

    return response;
  };
}

export const chatListWithMessagesState = new ChatListWithMessagesState();
