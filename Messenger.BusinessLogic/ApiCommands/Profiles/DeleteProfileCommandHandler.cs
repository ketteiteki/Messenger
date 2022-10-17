using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.BusinessLogic.Services;
using Messenger.Domain.Constants;
using Messenger.Services;

namespace Messenger.BusinessLogic.ApiCommands.Profiles;

public class DeleteProfileCommandHandler : IRequestHandler<DeleteProfileCommand, Result<UserDto>>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;

	public DeleteProfileCommandHandler(DatabaseContext context, IFileService fileService)
	{
		_context = context;
		_fileService = fileService;
	}
	
	public async Task<Result<UserDto>> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
	{
		var user = await _context.Users.FindAsync(request.RequesterId);

		if (user == null) return new Result<UserDto>(new DbEntityNotFoundError("User not found")); 

		if (user.AvatarLink != null)
		{
			_fileService.DeleteFile(Path.Combine(BaseDirService.GetPathWwwRoot(), user.AvatarLink.Split("/")[^1]));
			user.AvatarLink = null;
		}
		
		_context.Users.Remove(user);
		await _context.SaveChangesAsync(cancellationToken);

		return new Result<UserDto>(new UserDto(user));
	}
}