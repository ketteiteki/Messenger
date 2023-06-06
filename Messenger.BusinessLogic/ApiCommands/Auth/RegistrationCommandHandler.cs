using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models.Responses;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Constants;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
using Messenger.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Auth;

public class RegistrationCommandHandler : IRequestHandler<RegistrationCommand, Result<AuthorizationResponse>>
{
	private readonly DatabaseContext _context;
	private readonly IHashService _hashService;

	public RegistrationCommandHandler(DatabaseContext context, IHashService hashService)
	{
		_context = context;
		_hashService = hashService;
	}
	
	public async Task<Result<AuthorizationResponse>> Handle(RegistrationCommand request, CancellationToken cancellationToken)
	{
		var isUserByNicknameExists = await _context.Users.AnyAsync(u => u.Nickname == request.Nickname, cancellationToken);
		
		if (isUserByNicknameExists)
		{
			return new Result<AuthorizationResponse>(new AuthenticationError("User already exists"));
		}

		var hmac512CryptoHash = _hashService.Hmacsha512CryptoHash(request.Password, out var salt);

		var newUser = new UserEntity(
			request.DisplayName,
			request.Nickname,
			bio: null,
			avatarFileName: null,
			passwordHash: hmac512CryptoHash,
			passwordSalt: salt);

		_context.Users.Add(newUser);
		await _context.SaveChangesAsync(cancellationToken);

		var isDotnetChatCreated = await _context.Chats.AnyAsync(x => x.Id == SeedDataConstants.DotnetChatId, cancellationToken);
		var isDotnetFloodChatCreated = await _context.Chats.AnyAsync(x => x.Id == SeedDataConstants.DotnetFloodChatId, cancellationToken);

		if (!isDotnetChatCreated && !isDotnetFloodChatCreated)
		{
			var dotnetChat = new ChatEntity(
				title: "DotNetRuChat",
				name: "DotNetRuChat",
				type: ChatType.Conversation,
				ownerId: null,
				avatarFileName: "dotnetchat.jpg",
				lastMessageId: null)
			{
				Id = SeedDataConstants.DotnetChatId
			};
		
			var dotnetFloodChat = new ChatEntity(
				title: ".NET Talks",
				name: "dotnettalks",
				type: ChatType.Conversation,
				ownerId: null,
				avatarFileName: "dotnettalkschat.jpg",
				lastMessageId: null)
			{
				Id = SeedDataConstants.DotnetFloodChatId
			};
			
			_context.Chats.AddRange(dotnetChat, dotnetFloodChat);
		}

		var userChatForDotnetChat = new ChatUserEntity(
			newUser.Id,
			SeedDataConstants.DotnetChatId,
			true,
			muteDateOfExpire: null);
		
		var userChatForDotnetFloodChat = new ChatUserEntity(
			newUser.Id,
			SeedDataConstants.DotnetFloodChatId,
			true,
			muteDateOfExpire: null);
			
		_context.ChatUsers.AddRange(userChatForDotnetChat, userChatForDotnetFloodChat);
		await _context.SaveChangesAsync(cancellationToken);
		
		var authorizationResponse = new AuthorizationResponse(
			newUser.Id,
			newUser.DisplayName,
			newUser.Nickname,
			newUser.Bio,
			avatarLink: null);

		return new Result<AuthorizationResponse>(authorizationResponse);
	}
}