using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Microsoft.AspNetCore.Http;

namespace Messenger.BusinessLogic.ApiCommands.Chats;

public record CreateChatCommand(
    Guid RequesterId,
    string Name,
    string Title,
    ChatType Type,
    IFormFile AvatarFile
    ) : IRequest<Result<ChatDto>>;