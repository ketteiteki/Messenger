using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Auth;

public class RemoveSessionCommandHandler : IRequestHandler<RemoveSessionCommand, Result<SessionDto>>
{
    private readonly DatabaseContext _context;

    public RemoveSessionCommandHandler(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<Result<SessionDto>> Handle(RemoveSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.Sessions
            .FirstOrDefaultAsync(s => s.Id == request.SessionId && s.UserId == request.RequesterId, cancellationToken);

        if (session == null)
        {
            return new Result<SessionDto>(new DbEntityNotFoundError("Session not found"));
        }

        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync(cancellationToken);
        
        return new Result<SessionDto>(new SessionDto(session));
    }
}