using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Auth;

public record TerminateSessionCommand(Guid RequesterId, Guid SessionId) : IRequest<Result<UserSessionDto>>;