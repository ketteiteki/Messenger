using MediatR;
using Messenger.BusinessLogic.Models.Responses;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Auth;

public record AuthorizationCommand(
    Guid RequesterId,
    string AuthorizationToken) 
    : IRequest<Result<AuthorizationResponse>>;
