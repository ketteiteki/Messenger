using MediatR;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Entities;
using Messenger.Services;

namespace Messenger.BusinessLogic.ApiQueries.Users;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<User>>
{
	private readonly DatabaseContext _context;

	public GetUserQueryHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<Result<User>> Handle(GetUserQuery request, CancellationToken cancellationToken)
	{
		var findUser = await _context.Users.FindAsync(request.UserId, cancellationToken);

		if (findUser == null) return new Result<User>(new DbEntityNotFoundError("User not found"));

		return new Result<User>(findUser);
	}
}