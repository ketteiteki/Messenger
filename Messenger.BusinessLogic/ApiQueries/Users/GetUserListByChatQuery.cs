using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Users;

public record GetUserListByChatQuery(
        Guid RequesterId,
        Guid ChatId, 
        int Limit,
        int Page
    ) 
    : IRequest<Result<List<UserDto>>>;