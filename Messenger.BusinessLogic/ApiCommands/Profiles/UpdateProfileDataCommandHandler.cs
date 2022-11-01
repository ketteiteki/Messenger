using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Profiles;

public class UpdateProfileDataCommandHandler : IRequestHandler<UpdateProfileDataCommand, Result<UserDto>>
{
	private readonly DatabaseContext _context;

	public UpdateProfileDataCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<Result<UserDto>> Handle(UpdateProfileDataCommand request, CancellationToken cancellationToken)
	{
		var user = await _context.Users.FirstAsync(u => u.Id == request.RequesterId, cancellationToken);
		
		var userByNickname =  await _context.Users
			.FirstOrDefaultAsync(u => u.Nickname == request.Nickname, cancellationToken);

		if (userByNickname != null)
		{
			return new Result<UserDto>(new DbEntityExistsError("User with this nickname already exists")); 
		}

		user.Nickname = request.Nickname;
		user.DisplayName = request.DisplayName;
		user.Bio = request.Bio;

		_context.Users.Update(user);
		await _context.SaveChangesAsync(cancellationToken);

		return new Result<UserDto>(new UserDto(user));
	}
}