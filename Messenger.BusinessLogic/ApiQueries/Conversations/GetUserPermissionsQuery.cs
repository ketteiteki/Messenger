using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Conversations;

public record GetUserPermissionsQuery(Guid ChatId, Guid UserId) : IRequest<Result<PermissionDto>>;