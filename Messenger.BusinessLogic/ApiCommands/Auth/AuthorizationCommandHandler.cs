using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models.Responses;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Constants;
using Messenger.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Messenger.BusinessLogic.ApiCommands.Auth;

public class AuthorizationCommandHandler : IRequestHandler<AuthorizationCommand, Result<AuthorizationResponse>>
{
	private readonly DatabaseContext _context;
	private readonly ITokenService _tokenService;
	private readonly IConfiguration _configuration;
	private readonly IBlobServiceSettings _blobServiceSettings;

	public AuthorizationCommandHandler(
		DatabaseContext context,
		ITokenService tokenService,
		IConfiguration configuration,
		IBlobServiceSettings blobServiceSettings)
	{
		_context = context;
		_tokenService = tokenService;
		_configuration = configuration;
		_blobServiceSettings = blobServiceSettings;
	}
	
	public async Task<Result<AuthorizationResponse>> Handle(AuthorizationCommand request, CancellationToken cancellationToken)
	{
		var accessTokenSignKey = _configuration[AppSettingConstants.MessengerJwtSettingsSecretAccessTokenKey];
		var accessTokenLifeTimeMinutes = _configuration[AppSettingConstants.MessengerAccessTokenLifetimeMinutes];
		
		if (!_tokenService.TryValidateAccessToken(request.AuthorizationToken, accessTokenSignKey, out var validatedJwtToken))
		{
			return new Result<AuthorizationResponse>(new AuthenticationError("Incorrect token"));
		}

		var requesterGuid = new Guid(validatedJwtToken.Claims.First(c => c.Type == ClaimConstants.Id).Value);

		var requester = await _context.Users.FirstAsync(u => u.Id == requesterGuid, cancellationToken);

		var session = await _context.Sessions
			.FirstOrDefaultAsync(s => s.AccessToken == request.AuthorizationToken && 
			                          s.UserId == requesterGuid, cancellationToken);

		if (session == null)
		{
			return new Result<AuthorizationResponse>(new AuthenticationError("Access token is not linked to any session"));
		}
		
		var accessToken = _tokenService.CreateAccessToken(requester, accessTokenSignKey, int.Parse(accessTokenLifeTimeMinutes));

		session.UpdateAccessToken(accessToken);
		
		await _context.SaveChangesAsync(cancellationToken);

		var avatarLink = requester.AvatarFileName != null
			? $"{_blobServiceSettings.MessengerBlobAccess}/{requester.AvatarFileName}"
			: null;
		
		var authorizationResponse = new AuthorizationResponse(
			accessToken,
			session.RefreshToken,
			requester.Id,
			requester.DisplayName,
			requester.Nickname,
			requester.Bio,
			avatarLink);
		
		return new Result<AuthorizationResponse>(authorizationResponse);
	}
}
