using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models.RequestResponse;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Constants;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Messenger.BusinessLogic.ApiCommands.Auth;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
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
	
	public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
	{
		var user = await _context.Users.FirstOrDefaultAsync(u => u.NickName == request.NickName, cancellationToken);
		if (user == null) return new Result<LoginResponse>(new AuthenticationError("User does not exists"));

		var f = _hashService.Hmacsha512CryptoHashWithSalt(request.Password, user.PasswordSalt);
		
		if (user.PasswordHash != f)
			return new Result<LoginResponse>(new AuthenticationError("Password is wrong"));
		
		var accessToken = _tokenService.CreateAccessToken(user, 
			_configuration[AppSettingConstants.MessengerJwtSettingsSecretAccessTokenKey]);

		return new Result<LoginResponse>(new LoginResponse(user, accessToken));
	}
}