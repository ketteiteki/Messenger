using MediatR;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Services;

namespace Messenger.BusinessLogic.Profiles.Queries;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, UserDto>
{
	private readonly DatabaseContext _context;

	public GetProfileQueryHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<UserDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
	{
		var user = await _context.Users.FindAsync(request.ProfileId);

		if (user == null) throw new DbEntityNotFoundException("User not found");

		return new UserDto(user);
	}
}