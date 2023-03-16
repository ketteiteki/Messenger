using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models.Responses;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Constants;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Messenger.BusinessLogic.ApiCommands.Auth;

public class AuthorizationCommandHandler : IRequestHandler<AuthorizationCommand, Result<AuthorizationResponse>>
{
	private readonly DatabaseContext _context;
	private readonly ITokenService _tokenService;
	private readonly IConfiguration _configuration;

	public AuthorizationCommandHandler(DatabaseContext context, ITokenService tokenService, IConfiguration configuration)
	{
		_context = context;
		_tokenService = tokenService;
		_configuration = configuration;
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

		return new Result<AuthorizationResponse>(new AuthorizationResponse(requester,accessToken,session.RefreshToken));
	}
}
