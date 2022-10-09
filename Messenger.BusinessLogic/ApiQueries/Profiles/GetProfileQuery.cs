using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Profiles;

public record GetProfileQuery(
	Guid ProfileId) 
	: IRequest<Result<UserDto>>;