using System.Security.Authentication;
using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models.RequestResponse;
using Messenger.Domain.Constants;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Auth.Queries;

public class AuthorizationCommandHandler : IRequestHandler<AuthorizationCommand, AuthorizationResponse>
{
	private readonly DatabaseContext _context;
	private readonly ITokenService _tokenService;

	public AuthorizationCommandHandler(DatabaseContext context, ITokenService tokenService)
	{
		_context = context;
		_tokenService = tokenService;
	}
	
	public async Task<AuthorizationResponse> Handle(AuthorizationCommand request, CancellationToken cancellationToken)
	{
		var validatedToken = _tokenService.ValidateAccessToken(request.AuthorizationToken);
		if (validatedToken == null) throw new AuthenticationException("Incorrect token");

		var claimId = validatedToken.Claims.First(c => c.Type == ClaimConstants.Id);

		var findUser = await _context.Users.FirstAsync(u => u.Id.ToString() == claimId.Value, cancellationToken);
		if (findUser == null) throw new DbEntityNotFoundException("User not found");

		var newAccessToken = _tokenService.CreateAccessToken(findUser);
		
		return new AuthorizationResponse(findUser, newAccessToken);
	}
}