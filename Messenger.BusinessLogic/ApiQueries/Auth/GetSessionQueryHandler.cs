using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiQueries.Auth;

public class GetSessionQueryHandler : IRequestHandler<GetSessionQuery, Result<SessionDto>>
{
    private readonly DatabaseContext _context;

    public GetSessionQueryHandler(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<Result<SessionDto>> Handle(GetSessionQuery request, CancellationToken cancellationToken)
    {
        var session = await _context.Sessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => 
                s.AccessToken == request.AccessToken &&
                s.UserId == request.RequesterId, cancellationToken);

        if (session == null)
        {
            return new Result<SessionDto>(new DbEntityNotFoundError("Session not found"));
        }
        
        return new Result<SessionDto>(new SessionDto(session));
    }
}