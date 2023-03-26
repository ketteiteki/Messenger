using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiQueries.Users;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<UserDto>>
{
	private readonly DatabaseContext _context;
	private readonly IBlobServiceSettings _blobServiceSettings;

	public GetUserQueryHandler(
		DatabaseContext context,
		IBlobServiceSettings blobServiceSettings)
	{
		_context = context;
		_blobServiceSettings = blobServiceSettings;
	}
	
	public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
	{
		var user = await _context.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

		if (user == null)
		{
			return new Result<UserDto>(new DbEntityNotFoundError("User not found"));
		}

		var avatarLink = user.AvatarFileName != null
			? $"{_blobServiceSettings.MessengerBlobAccess}/{user.AvatarFileName}"
			: null;
		
		var userDto = new UserDto(
			user.Id,
			user.DisplayName,
			user.Nickname,
			user.Bio,
			avatarLink);
		
		return new Result<UserDto>(userDto);
	}
}