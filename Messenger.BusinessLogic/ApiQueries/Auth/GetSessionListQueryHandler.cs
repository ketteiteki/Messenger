using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiQueries.Auth;

public class GetSessionListQueryHandler : IRequestHandler<GetSessionListQuery, Result<List<SessionDto>>>
{
    private readonly DatabaseContext _context;

    public GetSessionListQueryHandler(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<Result<List<SessionDto>>> Handle(GetSessionListQuery request, CancellationToken cancellationToken)
    {
        var sessionList = await _context.Sessions
            .AsNoTracking()
            .Where(s => s.UserId == request.RequesterId)
            .Select(s => new SessionDto(s))
            .ToListAsync(cancellationToken);

        return new Result<List<SessionDto>>(sessionList);
    }
}