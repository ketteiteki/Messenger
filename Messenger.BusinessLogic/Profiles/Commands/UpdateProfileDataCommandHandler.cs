using MediatR;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Profiles.Commands;

public class UpdateProfileDataCommandHandler : IRequestHandler<UpdateProfileDataCommand, UserDto>
{
	private readonly DatabaseContext _context;

	public UpdateProfileDataCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<UserDto> Handle(UpdateProfileDataCommand request, CancellationToken cancellationToken)
	{
		var user = await _context.Users.FirstAsync(u => u.Id == request.RequestorId, cancellationToken);
		
		var userByNickName =  await _context.Users
			.FirstOrDefaultAsync(u => u.NickName == request.Nickname, cancellationToken);

		if (userByNickName != null)
			throw new DbEntityExistsException("User with this nickname already exists");

		user.NickName = request.Nickname;
		user.DisplayName = request.DisplayName;
		user.Bio = request.Bio;

		_context.Users.Update(user);
		await _context.SaveChangesAsync(cancellationToken);

		return new UserDto(user);
	}
}