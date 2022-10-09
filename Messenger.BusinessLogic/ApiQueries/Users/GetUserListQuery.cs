using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Users;

public record GetUserListQuery(
	string SearchText,
	int Count,
	int Page) 
	: IRequest<Result<List<UserDto>>>;