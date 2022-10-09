using MediatR;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Entities;

namespace Messenger.BusinessLogic.ApiQueries.Users;

public record GetUserQuery(
		Guid UserId) 
	: IRequest<Result<User>>;