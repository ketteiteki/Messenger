using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models.Responses;
using Messenger.BusinessLogic.Responses;
using Messenger.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Auth;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthorizationResponse>>
{
	private readonly DatabaseContext _context;
	private readonly IHashService _hashService;
	private readonly IBlobServiceSettings _blobServiceSettings;

	public LoginCommandHandler(DatabaseContext context,
		IHashService hashService,
		IBlobServiceSettings blobServiceSettings)
	{
		_context = context;
		_hashService = hashService;
		_blobServiceSettings = blobServiceSettings;
	}
	
	public async Task<Result<AuthorizationResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
	{
		var requester = await _context.Users
			.FirstOrDefaultAsync(u => u.Nickname == request.Nickname, cancellationToken);
		
		if (requester == null)
		{
			return new Result<AuthorizationResponse>(new AuthenticationError("User does not exists"));
		}

		var hmac512CryptoHash = _hashService.Hmacsha512CryptoHashWithSalt(request.Password, requester.PasswordSalt);

		if (requester.PasswordHash != hmac512CryptoHash)
		{
			return new Result<AuthorizationResponse>(new AuthenticationError("Password is wrong"));
		}
		
		await _context.SaveChangesAsync(cancellationToken);
		
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