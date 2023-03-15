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

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, Result<AuthorizationResponse>>
{
    private readonly DatabaseContext _context;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public RefreshCommandHandler(DatabaseContext context, ITokenService tokenService, IConfiguration configuration)
    {
        _context = context;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<Result<AuthorizationResponse>> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var accessTokenSignKey = _configuration[AppSettingConstants.MessengerJwtSettingsSecretAccessTokenKey];
        var accessTokenLifeTimeMinutes = _configuration[AppSettingConstants.MessengerAccessTokenLifetimeMinutes];
        var refreshTokenLifetimeDays = _configuration[AppSettingConstants.MessengerRefreshTokenLifetimeDays];
        
        var session = await _context.Sessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.RefreshToken == request.RefreshToken, cancellationToken);

        if (session == null)
        {
            return new Result<AuthorizationResponse>(new AuthenticationError("Refresh token not found"));
        }
        
        if (session.IsExpired)
        {
            return new Result<AuthorizationResponse>(new AuthenticationError("Refresh token is expired"));
        }

        var accessToken = _tokenService.CreateAccessToken(session.User, accessTokenSignKey, int.Parse(accessTokenLifeTimeMinutes));

        var newSessionExpiresAt = DateTime.UtcNow.AddDays(int.Parse(refreshTokenLifetimeDays));
        
        var newSession = new SessionEntity(
            accessToken: accessToken,
            userId: session.UserId,
            ip: request.Ip,
            userAgent: request.UserAgent,
            expiresAt: newSessionExpiresAt);

        _context.Sessions.Remove(session);
        _context.Sessions.Add(newSession);
        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(newSession).Reference(s => s.User).LoadAsync(cancellationToken);
        
        return new Result<AuthorizationResponse>(new AuthorizationResponse(newSession.User, accessToken, newSession.RefreshToken));
    }
}