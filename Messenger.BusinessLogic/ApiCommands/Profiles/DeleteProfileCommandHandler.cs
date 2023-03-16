using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Profiles;

public class DeleteProfileCommandHandler : IRequestHandler<DeleteProfileCommand, Result<UserDto>>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;
	private readonly IBaseDirService _baseDirService;

	public DeleteProfileCommandHandler(
		DatabaseContext context, 
		IFileService fileService, 
		IBaseDirService baseDirService)
	{
		_context = context;
		_fileService = fileService;
		_baseDirService = baseDirService;
	}
	
	public async Task<Result<UserDto>> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
	{
		var requester = await _context.Users
			.Include(u => u.ChatUsers)
			.FirstAsync(u => u.Id == request.RequesterId, cancellationToken);

		if (requester.AvatarLink != null)
		{
			var pathWwwRoot = _baseDirService.GetPathWwwRoot();
			var avatarFileName = requester.AvatarLink.Split("/")[^1];

			var avatarFilePath = Path.Combine(pathWwwRoot, avatarFileName);
			
			_fileService.DeleteFile(avatarFilePath);
			
			requester.UpdateAvatarLink(null);
		}
		
		_context.Users.Remove(requester);
		
		await _context.SaveChangesAsync(cancellationToken);

		return new Result<UserDto>(new UserDto(requester));
	}
}