using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Auth;

public class TerminateSessionCommandHandler : IRequestHandler<TerminateSessionCommand, Result<UserSessionDto>>
{
    private readonly DatabaseContext _context;

    public TerminateSessionCommandHandler(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<Result<UserSessionDto>> Handle(TerminateSessionCommand request, CancellationToken cancellationToken)
    {
        var userSession = await _context.UserSessions
            .FirstOrDefaultAsync(x => x.Id == request.SessionId && x.UserId == request.RequesterId, cancellationToken);

        if (userSession == null)
        {
            return new Result<UserSessionDto>(new DbEntityNotFoundError("User session does not exist"));
        }

        _context.UserSessions.Remove(userSession);
        await _context.SaveChangesAsync(cancellationToken);

        var userSessionDto = new UserSessionDto(userSession.Id, userSession.CreatedAt);
        
        return new Result<UserSessionDto>(userSessionDto);
    }
}