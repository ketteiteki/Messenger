using MediatR;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Services;

namespace Messenger.BusinessLogic.Profiles.Commands;

public class DeleteProfileCommandHandler : IRequestHandler<DeleteProfileCommand, UserDto>
{
	private readonly DatabaseContext _context;

	public DeleteProfileCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<UserDto> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
	{
		var user = await _context.Users.FindAsync(request.RequestorId);

		if (user == null) throw new DbEntityNotFoundException("User not found");

		_context.Users.Remove(user);
		await _context.SaveChangesAsync(cancellationToken);

		return new UserDto(user);
	}
}