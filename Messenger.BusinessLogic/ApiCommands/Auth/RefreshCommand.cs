using MediatR;
using Messenger.BusinessLogic.Models.Responses;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Auth;

public record RefreshCommand(
        string UserAgent, 
        string Ip,
        Guid RefreshToken) 
    : IRequest<Result<AuthorizationResponse>>;