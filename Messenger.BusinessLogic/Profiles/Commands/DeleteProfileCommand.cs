using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Profiles.Commands;

public class DeleteProfileCommand : IRequest<UserDto>
{
	public Guid RequestorId { get; set; }
}