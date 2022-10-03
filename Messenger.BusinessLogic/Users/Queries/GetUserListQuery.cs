using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Users.Queries;

public class GetUserListQuery : IRequest<List<UserDto>>
{
	public string SearchText { get; set; }

	public int Count { get; set; } = 10;

	public int Page { get; set; } = 1;
}