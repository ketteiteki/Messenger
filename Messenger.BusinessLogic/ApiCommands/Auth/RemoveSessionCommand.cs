using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Auth;

public record RemoveSessionCommand(
        Guid RequesterId,
        Guid SessionId)
    : IRequest<Result<SessionDto>>;