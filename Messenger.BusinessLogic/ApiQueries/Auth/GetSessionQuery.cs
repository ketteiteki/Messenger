using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Auth;

public record GetSessionQuery(
        Guid RequesterId,
        string AccessToken) 
    : IRequest<Result<SessionDto>>;