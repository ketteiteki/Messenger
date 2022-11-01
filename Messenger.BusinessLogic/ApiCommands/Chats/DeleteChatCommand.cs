using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Chats;

public record DeleteChatCommand(
    Guid RequesterId,
    Guid ChatId) 
    : IRequest<Result<ChatDto>>; 