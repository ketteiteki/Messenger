using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Chats;

public record GetChatListBySearchQuery(
        Guid RequesterId,
        string SearchText) 
    : IRequest<Result<List<ChatDto>>>;