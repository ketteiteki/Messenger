using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Auth;

public record GetSessionListQuery(
        Guid RequesterId) 
    : IRequest<Result<List<SessionDto>>>;