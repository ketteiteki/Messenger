using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiQueries.Users;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<UserDto>>
{
	private readonly DatabaseContext _context;

	public GetUserQueryHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
	{
		var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

		if (user == null) return new Result<UserDto>(new DbEntityNotFoundError("User not found"));

		return new Result<UserDto>(new UserDto(user));
	}
}