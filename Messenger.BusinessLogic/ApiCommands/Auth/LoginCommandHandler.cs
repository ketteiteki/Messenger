using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models.Responses;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Constants;
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

	public LoginCommandHandler(DatabaseContext context, IHashService hashService, ITokenService tokenService, IConfiguration configuration)
	{
		_context = context;
		_hashService = hashService;
		_tokenService = tokenService;
		_configuration = configuration;
	}
	
	public async Task<Result<AuthorizationResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
	{
		var requester = await _context.Users.FirstOrDefaultAsync(u => u.NickName == request.NickName, cancellationToken);
		if (requester == null) return new Result<AuthorizationResponse>(new AuthenticationError("User does not exists"));

		var f = _hashService.Hmacsha512CryptoHashWithSalt(request.Password, requester.PasswordSalt);
		
		if (requester.PasswordHash != f)
			return new Result<AuthorizationResponse>(new AuthenticationError("Password is wrong"));
		
		var accessToken = _tokenService.CreateAccessToken(
			requester, _configuration[AppSettingConstants.MessengerJwtSettingsSecretAccessTokenKey]);

		return new Result<AuthorizationResponse>(new AuthorizationResponse(requester, accessToken));
	}
}