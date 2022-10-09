using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models.RequestResponse;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Entities;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Auth;

public class RegistrationCommandHandler : IRequestHandler<RegistrationCommand, Result<RegistrationResponse>>
{
	private readonly DatabaseContext _context;
	private readonly IHashService _hashService;
	private readonly ITokenService _tokenService;

	public RegistrationCommandHandler(DatabaseContext context, IHashService hashService, ITokenService tokenService)
	{
		_context = context;
		_hashService = hashService;
		_tokenService = tokenService;
	}
	
	public async Task<Result<RegistrationResponse>> Handle(RegistrationCommand request, CancellationToken cancellationToken)
	{
		var findUser = await _context.Users.FirstOrDefaultAsync(u => u.NickName == request.Nickname, cancellationToken);
		if (findUser != null) return new Result<RegistrationResponse>(new AuthenticationError("User already exists"));

		var passwordHash = _hashService.HMACSHA512CryptoHash(request.Password, out var salt);

		var newUser = new User(
			displayName: request.DisplayName,
			nickName: request.Nickname,
			passwordHash: passwordHash,
			passwordSalt: salt,
			bio: null,
			avatarLink: null);

		_context.Users.Add(newUser);
		await _context.SaveChangesAsync(cancellationToken);

		var accessToken = _tokenService.CreateAccessToken(newUser);
		
		return new Result<RegistrationResponse>(new RegistrationResponse(newUser, accessToken));
	}
}