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

  constructor() {
    makeAutoObservable(
      this,
      {},
      {
        deep: true,
      }
    );
  }

  public setLastMessage = (message: IMessageDto) => {
    const item = this.data.find((c) => c.chat.id === message.chatId);

    if (item === undefined) return;

    item.chat.lastMessageId = message.id;
    item.chat.lastMessageText = message.text;
    item.chat.lastMessageAuthorDisplayName = message.ownerDisplayName;
    item.chat.lastMessageDateOfCreate = message.dateOfCreate;
  };

  public addMessageInData = (message: IMessageDto) => {
    console.log(message);
    const item = this.data.find((c) => c.chat.id === message.chatId);
    console.log(item);
    const itemInSearchData = this.dataForSearchChats.find((c) => c.chat.id === message.chatId);
    console.log(itemInSearchData);

    item?.messages.push(message);
    itemInSearchData?.messages.push(message);
  };

  public updateMessageInData = (
    messageUpdateNotification: IMessageUpdateNotificationDto
  ) => {
    const item = this.data.find(
      (c) => c.chat.id === messageUpdateNotification.chatId
    );

    if (item === undefined) return;

    const message = item.messages.find(
      (x) => x.id === messageUpdateNotification.messageId
    );

    if (message === undefined) return;

    message.text = messageUpdateNotification.updatedText;
    message.isEdit = true;

    if (messageUpdateNotification.isLastMessage) {
      item.chat.lastMessageText = messageUpdateNotification.updatedText;
    }
  };

  public deleteMessageInData = (chatId: string, messageId: string) => {
    const dataItem = this.data.find(x => x.chat.id === chatId);

    const messageItemIndex = dataItem?.messages.findIndex(x => x.id === messageId);

    if (!messageItemIndex) return;

    dataItem?.messages.splice(messageItemIndex, 1);
  };

  public deleteMessageInDataByMessageDeleteNotification = (
    messageDeleteNotification: IMessageDeleteNotificationDto
  ) => {
    const item = this.data.find(
      (c) => c.chat.id === messageDeleteNotification.chatId
    );

    if (item === undefined) return;

    item.messages = item.messages.filter(
      (x) => x.id !== messageDeleteNotification.messageId
    );
  };

  public addChatInData = (chat: IChatDto, messages: IMessageDto[]) => {
    this.data.push({chat, messages});
  };

  public resetDataForSearchChats = () => {
    this.dataForSearchChats = [];
  };

  //api
  public getMessageListAsync = async (
    chatId: string,
    fromMessageDateTime: string | null
  ) => {
    const response = await MessagesApi.getMessageListAsync(
      chatId,
      fromMessageDateTime,
      40
    );

    if (response.status === 200) {
      runInAction(() => {
        const dataItem = this.data.find((i) => i.chat.id === chatId);
        const dataForSearchItem = this.dataForSearchChats.find(
          (i) => i.chat.id === chatId
        );

        const messages = response.data.reverse();

        dataItem?.messages.unshift(...messages);
        dataForSearchItem?.messages.unshift(...messages);
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
    const item = this.data.find((c) => c.chat.id === chatId);

    if (item === undefined) return;

    const message = item.messages.find((x) => x.id === messageId);

    if (message === undefined) return;
    message.text = text;

    const response = await MessagesApi.putUpdateMessageAsync(messageId, text);

    if (
      item.messages.length - 1 ===
      item.messages.findIndex((x) => x.id === messageId)
    ) {
      item.chat.lastMessageText = text;
    }

    return response;
  };

  public delDeleteMessageAsync = async (chatId: string, messageId: string) => {
    const item = this.data.find((c) => c.chat.id === chatId);

    if (item === undefined) return;

    console.log(item.messages.findIndex((x) => x.id === "43"));

    const messageIndex = item.messages.findIndex((x) => x.id === messageId);

    if (messageIndex === -1) return;

    item.messages.splice(messageIndex, 1);

    const response = await MessagesApi.delDeleteMessageAsync(messageId, true);

    return response;
  };

  public postCreateDialogAsync = async (userId: string) => {
    const response = await DialogApi.postCreateDialogAsync(userId);

    if (response.status === 200) {
      runInAction(() => {
        this.data.push({chat: response.data, messages: []});
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

        if (item === undefined) return;

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
