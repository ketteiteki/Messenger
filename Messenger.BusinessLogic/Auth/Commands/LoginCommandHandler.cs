using System.Security.Authentication;
using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models.RequestResponse;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
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
	
	public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
	{
		var user = await _context.Users.FirstOrDefaultAsync(u => u.NickName == request.NickName, cancellationToken);
		if (user == null) throw new AuthenticationException("User does not exists");

		var f = _hashService.HMACSHA512CryptoHashWithSalt(request.Password, user.PasswordSalt);
		
		if (user.PasswordHash != f)
			throw new AuthenticationException("Password is wrong");
		
		var accessToken = _tokenService.CreateAccessToken(user);

		return new LoginResponse(user, accessToken);
	}
}