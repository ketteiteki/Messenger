using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models.RequestResponse;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Constants;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Messenger.BusinessLogic.ApiQueries.Auth;

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
				    out JwtSecurityToken validatedJwtToken))
			throw new AuthenticationException("Incorrect token");

		var claimId = validatedJwtToken.Claims.First(c => c.Type == ClaimConstants.Id);

		var findUser = await _context.Users.FirstAsync(u => u.Id.ToString() == claimId.Value, cancellationToken);

		var newAccessToken = _tokenService.CreateAccessToken(findUser,
			_configuration[AppSettingConstants.MessengerJwtSettingsSecretAccessTokenKey]);
		
		return new Result<AuthorizationResponse>(new AuthorizationResponse(findUser, newAccessToken));
	}
}