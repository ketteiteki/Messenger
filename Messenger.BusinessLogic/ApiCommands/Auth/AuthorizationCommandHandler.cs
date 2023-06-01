using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models.Responses;
using Messenger.BusinessLogic.Responses;
using Messenger.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Auth;

public class AuthorizationCommandHandler : IRequestHandler<AuthorizationCommand, Result<AuthorizationResponse>>
{
	private readonly DatabaseContext _context;
	private readonly IBlobServiceSettings _blobServiceSettings;

	public AuthorizationCommandHandler(DatabaseContext context, IBlobServiceSettings blobServiceSettings)
	{
		_context = context;
		_blobServiceSettings = blobServiceSettings;
	}
	
	public async Task<Result<AuthorizationResponse>> Handle(AuthorizationCommand request, CancellationToken cancellationToken)
	{
		var requester = await _context.Users
			.AsNoTracking()
			.FirstAsync(u => u.Id == request.RequesterId, cancellationToken);

		var avatarLink = requester.AvatarFileName != null
			? $"{_blobServiceSettings.MessengerBlobAccess}/{requester.AvatarFileName}"
			: null;
		
		var authorizationResponse = new AuthorizationResponse(
			requester.Id,
			requester.DisplayName,
			requester.Nickname,
			requester.Bio,
			avatarLink);
		
		return new Result<AuthorizationResponse>(authorizationResponse);
	}
}
