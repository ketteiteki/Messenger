using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models.RequestResponse;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Constants;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiQueries.Auth;

public class AuthorizationCommandHandler : IRequestHandler<AuthorizationCommand, Result<AuthorizationResponse>>
{
	private readonly DatabaseContext _context;
	private readonly ITokenService _tokenService;

	public AuthorizationCommandHandler(DatabaseContext context, ITokenService tokenService)
	{
		_context = context;
		_tokenService = tokenService;
	}
	
	public async Task<Result<AuthorizationResponse>> Handle(AuthorizationCommand request, CancellationToken cancellationToken)
	{
		if (!_tokenService.TryValidateAccessToken(request.AuthorizationToken, out JwtSecurityToken validatedJwtToken))
			throw new AuthenticationException("Incorrect token");

		var claimId = validatedJwtToken.Claims.First(c => c.Type == ClaimConstants.Id);

		var findUser = await _context.Users.FirstAsync(u => u.Id.ToString() == claimId.Value, cancellationToken);

		var newAccessToken = _tokenService.CreateAccessToken(findUser);
		
		return new Result<AuthorizationResponse>(new AuthorizationResponse(findUser, newAccessToken));
	}
}