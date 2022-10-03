using MediatR;
using Messenger.BusinessLogic.Models;
using Microsoft.AspNetCore.Http;

namespace Messenger.BusinessLogic.Profiles.Commands;

public class UpdateProfileAvatarCommand : IRequest<UserDto>
{
	public Guid RequestorId { get; set; }
	
	public IFormFile? AvatarFile { get; set; }
}