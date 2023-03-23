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

public class RegistrationCommandHandler : IRequestHandler<RegistrationCommand, Result<AuthorizationResponse>>
{
	private readonly DatabaseContext _context;
	private readonly IHashService _hashService;
	private readonly ITokenService _tokenService;
	private readonly IConfiguration _configuration;

	public RegistrationCommandHandler(DatabaseContext context, IHashService hashService, ITokenService tokenService,
		IConfiguration configuration)
	{
		_context = context;
		_hashService = hashService;
		_tokenService = tokenService;
		_configuration = configuration;
	}
	
	public async Task<Result<AuthorizationResponse>> Handle(RegistrationCommand request, CancellationToken cancellationToken)
	{
		var accessTokenSignKey = _configuration[AppSettingConstants.MessengerJwtSettingsSecretAccessTokenKey];
		var accessTokenLifeTimeMinutes = _configuration[AppSettingConstants.MessengerAccessTokenLifetimeMinutes];
		var refreshTokenLifetimeDays = _configuration[AppSettingConstants.MessengerRefreshTokenLifetimeDays];
		
		var isUserByNicknameExists = await _context.Users.AnyAsync(u => u.Nickname == request.Nickname, cancellationToken);
		
		if (isUserByNicknameExists)
		{
			return new Result<AuthorizationResponse>(new AuthenticationError("User already exists"));
		}

		var hmac512CryptoHash = _hashService.Hmacsha512CryptoHash(request.Password, out var salt);

		var newUser = new UserEntity(
			request.DisplayName,
			request.Nickname,
			bio: null,
			avatarLink: null,
			passwordHash: hmac512CryptoHash,
			passwordSalt: salt);

		var accessToken = _tokenService.CreateAccessToken(newUser, accessTokenSignKey, int.Parse(accessTokenLifeTimeMinutes));

		var sessionExpiresAt = DateTime.UtcNow.AddDays(int.Parse(refreshTokenLifetimeDays));
		
		var session = new SessionEntity(
			newUser.Id,
			accessToken,
			request.Ip,
			request.UserAgent,
			sessionExpiresAt);

		_context.Users.Add(newUser);
		_context.Sessions.Add(session);
		await _context.SaveChangesAsync(cancellationToken);

		return new Result<AuthorizationResponse>(new AuthorizationResponse(newUser, accessToken, session.RefreshToken));
	}
}