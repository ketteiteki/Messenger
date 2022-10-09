using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Services;

namespace Messenger.BusinessLogic.ApiQueries.Profiles;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, Result<UserDto>>
{
	private readonly DatabaseContext _context;

	public GetProfileQueryHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task< Result<UserDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
	{
		var user = await _context.Users.FindAsync(request.ProfileId);

		if (user == null) return new Result<UserDto>(new DbEntityNotFoundError("User not found"));

		return new Result<UserDto>(new UserDto(user));
	}
}