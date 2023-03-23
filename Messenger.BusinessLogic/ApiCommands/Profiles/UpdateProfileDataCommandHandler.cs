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
		var isDataNotChanged = request.Nickname == null && request.DisplayName == null && request.Bio == null;
		
		if (isDataNotChanged)
		{
			return new Result<UserDto>(new BadRequestError("Data cannot all be null"));
		}
		
		var requester = await _context.Users.FirstAsync(u => u.Id == request.RequesterId, cancellationToken);

		var isNicknameChanged = request.Nickname != null && requester.Nickname != request.Nickname;
		
		if (isNicknameChanged)
		{
			var isUserByNicknameExists =  await _context.Users.AnyAsync(u => u.Nickname == request.Nickname, cancellationToken);

			if (isUserByNicknameExists)
			{
				return new Result<UserDto>(new DbEntityExistsError("User with this nickname already exists")); 
			}
			
			requester.UpdateNickname(request.Nickname);
		}

		if (request.DisplayName != null)
		{
			requester.UpdateDisplayName(request.DisplayName);
		}
		
		if (request.Bio != null)
		{
			requester.UpdateBio(request.Bio);
		}
		
		_context.Users.Update(requester);
		await _context.SaveChangesAsync(cancellationToken);

		return new Result<UserDto>(new UserDto(requester));
	}
}