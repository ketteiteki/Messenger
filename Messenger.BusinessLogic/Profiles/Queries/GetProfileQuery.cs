using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Profiles.Queries;

public class GetProfileQuery : IRequest<UserDto>
{
	public Guid ProfileId { get; set; }
}