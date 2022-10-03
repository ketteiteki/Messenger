using MediatR;
using Messenger.BusinessLogic.Exceptions;
using Messenger.Domain.Entities;
using Messenger.Services;

namespace Messenger.BusinessLogic.Users.Queries;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, User>
{
	private readonly DatabaseContext _context;

	public GetUserQueryHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<User> Handle(GetUserQuery request, CancellationToken cancellationToken)
	{
		var findUser = await _context.Users.FindAsync(request.UserId, cancellationToken);

		if (findUser == null) throw new DbEntityNotFoundException("User not found");

		return findUser;
	}
}