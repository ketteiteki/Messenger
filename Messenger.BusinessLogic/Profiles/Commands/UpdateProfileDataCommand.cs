using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Profiles.Commands;

public class UpdateProfileDataCommand : IRequest<UserDto>
{
	public Guid RequestorId { get; set; }

	public string DisplayName { get; set; }
    
	public string Nickname { get; set; }
	
	public string? Bio { get; set; }
}