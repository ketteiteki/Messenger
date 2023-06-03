using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiQueries.Auth;

public class GetSessionsQueryHandler : IRequestHandler<GetSessionsQuery, Result<List<UserSessionDto>>>
{
    private readonly DatabaseContext _context;

    public GetSessionsQueryHandler(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<Result<List<UserSessionDto>>> Handle(GetSessionsQuery request, CancellationToken cancellationToken)
    {
        var userSessions = await _context.UserSessions
            .Where(x => x.UserId == request.RequestId)
            .Select(x => new UserSessionDto(x.Id, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return new Result<List<UserSessionDto>>(userSessions);
    }
}