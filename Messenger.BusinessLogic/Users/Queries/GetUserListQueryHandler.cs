using System.Text.RegularExpressions;
using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Users.Queries;

public class GetUserListQueryHandler : IRequestHandler<GetUserListQuery, List<UserDto>>
{
	private readonly DatabaseContext _context;

	public GetUserListQueryHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<List<UserDto>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
	{
		return await _context.Users
			.AsNoTracking()
			.Where(u => Regex.IsMatch(u.NickName, request.SearchText, RegexOptions.IgnoreCase))
			.Skip((request.Page - 1) * request.Count)
			.Take(request.Count)
			.Select(u => new UserDto(u))
			.ToListAsync(cancellationToken);
	}
}