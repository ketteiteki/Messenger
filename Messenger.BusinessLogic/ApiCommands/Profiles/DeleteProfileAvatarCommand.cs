using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Profiles;

public record DeleteProfileAvatarCommand(
        Guid RequesterId) 
    : IRequest<Result<UserDto>>;