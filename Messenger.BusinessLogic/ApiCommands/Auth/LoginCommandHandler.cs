using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models.Responses;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Constants;
using Messenger.Domain.Entities;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Messenger.BusinessLogic.ApiCommands.Auth;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthorizationResponse>>
{
	private readonly DatabaseContext _context;
	private readonly IHashService _hashService;
	private readonly ITokenService _tokenService;
	private readonly IConfiguration _configuration;

	public LoginCommandHandler(DatabaseContext context,
		IHashService hashService,
		ITokenService tokenService,
		IConfiguration configuration)
	{
		_context = context;
		_hashService = hashService;
		_tokenService = tokenService;
		_configuration = configuration;
	}
	
	public async Task<Result<AuthorizationResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
	{
		var accessTokenSignKey = _configuration[AppSettingConstants.MessengerJwtSettingsSecretAccessTokenKey];
		var accessTokenLifeTimeMinutes = _configuration[AppSettingConstants.MessengerAccessTokenLifetimeMinutes];
		var refreshTokenLifetimeDays = _configuration[AppSettingConstants.MessengerRefreshTokenLifetimeDays];
		
		var requester = await _context.Users
			.Include(u => u.Sessions)
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
		
		var accessToken = _tokenService.CreateAccessToken(requester, accessTokenSignKey, int.Parse(accessTokenLifeTimeMinutes));

		var sessionExpiresAt = DateTime.UtcNow.AddDays(int.Parse(refreshTokenLifetimeDays));
		
		var session = new SessionEntity(
			accessToken: accessToken,
			userId: requester.Id,
			ip: request.Ip,
			userAgent: request.UserAgent,
			expiresAt: sessionExpiresAt);

		if (requester.Sessions.Count >= 7)
		{
			var lastExpiringSession = requester.Sessions.DistinctBy(s => s.CreateAt).First();
			
			requester.Sessions.Remove(lastExpiringSession);
		}
		
		_context.Sessions.Add(session);
		await _context.SaveChangesAsync(cancellationToken);
		
		return new Result<AuthorizationResponse>(new AuthorizationResponse(requester, accessToken, session.RefreshToken));
	}
}