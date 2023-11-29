using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Auth;

public record GetSessionsQuery(Guid RequestId) : IRequest<Result<List<UserSessionDto>>>;
