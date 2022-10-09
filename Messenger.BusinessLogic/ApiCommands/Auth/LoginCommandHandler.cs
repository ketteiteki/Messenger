using System.Security.Authentication;
using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models.RequestResponse;
using Messenger.BusinessLogic.Responses;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Auth;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
	private readonly DatabaseContext _context;
	private readonly IHashService _hashService;
	private readonly ITokenService _tokenService;

	public LoginCommandHandler(DatabaseContext context, IHashService hashService, ITokenService tokenService)
	{
		_context = context;
		_hashService = hashService;
		_tokenService = tokenService;
	}
	
	public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
	{
		var user = await _context.Users.FirstOrDefaultAsync(u => u.NickName == request.NickName, cancellationToken);
		if (user == null) return new Result<LoginResponse>(new AuthenticationError("User does not exists"));

		var f = _hashService.HMACSHA512CryptoHashWithSalt(request.Password, user.PasswordSalt);
		
		if (user.PasswordHash != f)
			return new Result<LoginResponse>(new AuthenticationError("Password is wrong"));
		
		var accessToken = _tokenService.CreateAccessToken(user);

		return new Result<LoginResponse>(new LoginResponse(user, accessToken));
	}
}