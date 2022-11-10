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
		if (!_tokenService.TryValidateAccessToken(request.AuthorizationToken,
			    _configuration[AppSettingConstants.MessengerJwtSettingsSecretAccessTokenKey],
			    out var validatedJwtToken))
		{
			return new Result<AuthorizationResponse>(new AuthenticationError("Incorrect token"));
		}

		var requesterId = new Guid(validatedJwtToken.Claims.First(c => c.Type == ClaimConstants.Id).Value);

		var requester = await _context.Users.FirstAsync(u => u.Id == requesterId, cancellationToken);

		var session = await _context.Sessions
			.FirstOrDefaultAsync(s => s.AccessToken == request.AuthorizationToken && 
			                          s.UserId == requesterId, cancellationToken);

		if (session == null)
		{
			return new Result<AuthorizationResponse>(new AuthenticationError("Access token is not linked to any session"));
		}
		
		var accessToken = _tokenService.CreateAccessToken(requester,
			_configuration[AppSettingConstants.MessengerJwtSettingsSecretAccessTokenKey],
			int.Parse(_configuration[AppSettingConstants.MessengerAccessTokenLifetimeMinutes]));

		session.AccessToken = accessToken;

		await _context.SaveChangesAsync(cancellationToken);
		
		return new Result<AuthorizationResponse>(new AuthorizationResponse(
			user: requester, 
			accessToken: accessToken,
			refreshToken: session.RefreshToken));
	}
}
