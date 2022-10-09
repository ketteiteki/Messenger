using System.Text.RegularExpressions;
using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiQueries.Users;

public class GetUserListQueryHandler : IRequestHandler<GetUserListQuery, Result<List<UserDto>>>
{
	private readonly DatabaseContext _context;

	public GetUserListQueryHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<Result<List<UserDto>>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
	{
		var users = await _context.Users
			.AsNoTracking()
			.Where(u => Regex.IsMatch(u.NickName, request.SearchText, RegexOptions.IgnoreCase))
			.Skip((request.Page - 1) * request.Count)
			.Take(request.Count)
			.Select(u => new UserDto(u))
			.ToListAsync(cancellationToken);
		
		return new Result<List<UserDto>>(users);
	}
}