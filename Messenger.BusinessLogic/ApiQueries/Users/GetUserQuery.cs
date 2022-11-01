using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Users;

public record GetUserQuery(
		Guid UserId) 
	: IRequest<Result<UserDto>>;