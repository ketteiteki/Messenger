using MediatR;
using Messenger.Domain.Entities;

namespace Messenger.BusinessLogic.Users.Queries;

public class GetUserQuery : IRequest<User>
{
	public Guid UserId { get; set; }
}