using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiQueries.Conversations;

public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, Result<PermissionDto>>
{
    private readonly DatabaseContext _context;

    public GetUserPermissionsQueryHandler(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<Result<PermissionDto>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        var userPermissions = await _context.ChatUsers
            .Where(x => x.UserId == request.UserId && x.ChatId == request.ChatId)
            .Select(c => new PermissionDto(c))
            .FirstOrDefaultAsync(cancellationToken);

        if (userPermissions == null)
        {
            return new Result<PermissionDto>(new DbEntityNotFoundError("user permissions not found"));
        }
        
        return new Result<PermissionDto>(userPermissions);
    }
}