using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.BusinessLogic.Services;
using Messenger.Domain.Constants;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Messenger.BusinessLogic.ApiCommands.Chats;

public class UpdateChatAvatarCommandHandler : IRequestHandler<UpdateChatAvatarCommand, Result<ChatDto>>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;
	private readonly IConfiguration _configuration;
	
	public UpdateChatAvatarCommandHandler(DatabaseContext context, IFileService fileService, IConfiguration configuration)
	{
		_context = context;
		_fileService = fileService;
		_configuration = configuration;
	}
	
	public async Task<Result<ChatDto>> Handle(UpdateChatAvatarCommand request, CancellationToken cancellationToken)
	{
		var messengerDomainName = _configuration[AppSettingConstants.MessengerDomainName];
		
		var chatUserByRequester = await _context.ChatUsers
			.Include(c => c.Chat)
			.ThenInclude(c => c.Owner)
			.Include(c => c.Role)
			.FirstOrDefaultAsync(c => 
				c.UserId == request.RequesterId && 
				c.ChatId == request.ChatId &&
				c.Chat.Type != ChatType.Dialog, cancellationToken);

		if (chatUserByRequester == null)
		{
			return new Result<ChatDto>(new DbEntityNotFoundError("No requester in the chat"));
		}

		if ((chatUserByRequester.Role == null &&
		     chatUserByRequester.Chat.OwnerId != request.RequesterId)  || 
		    (chatUserByRequester.Role is { CanChangeChatData: false } &&
		     chatUserByRequester.Chat.OwnerId != request.RequesterId))
		{
			return new Result<ChatDto>(new ForbiddenError("It is forbidden to update someone else's chat"));
		}

		if (chatUserByRequester.Chat.AvatarLink != null)
		{
			var pathWwwRoot = BaseDirService.GetPathWwwRoot();
			var avatarFileName = chatUserByRequester.Chat.AvatarLink.Split("/")[^1];

			var avatarFilePath = Path.Combine(pathWwwRoot, avatarFileName);
				
			_fileService.DeleteFile(avatarFilePath);
				
			chatUserByRequester.Chat.UpdateAvatarLink(null);
		}
			
		if (request.AvatarFile != null)
		{
			var pathWwwRoot = BaseDirService.GetPathWwwRoot();
				
			var avatarLink = await _fileService.CreateFileAsync(pathWwwRoot, request.AvatarFile, messengerDomainName);
				
			chatUserByRequester.Chat.UpdateAvatarLink(avatarLink);
		}
			
		_context.Chats.Update(chatUserByRequester.Chat);
			
		await _context.SaveChangesAsync(cancellationToken);

		var chatDto = new ChatDto
		{
			Id = chatUserByRequester.ChatId,
			Name = chatUserByRequester.Chat.Name,
			Title = chatUserByRequester.Chat.Title,
			Type = chatUserByRequester.Chat.Type,
			AvatarLink = chatUserByRequester.Chat.AvatarLink,
			LastMessageId = chatUserByRequester.Chat.LastMessageId,
			LastMessageText = chatUserByRequester.Chat.LastMessage?.Text,
			LastMessageAuthorDisplayName =
				chatUserByRequester.Chat.LastMessage is { Owner: { } }
				? chatUserByRequester.Chat.LastMessage.Owner.DisplayName
				: null,
			LastMessageDateOfCreate = chatUserByRequester.Chat.LastMessage?.DateOfCreate,
			IsOwner = chatUserByRequester.Chat.OwnerId == request.RequesterId,
			IsMember = true,
			MuteDateOfExpire = chatUserByRequester.MuteDateOfExpire,
			BanDateOfExpire = null,
			RoleUser = chatUserByRequester.Role != null ? new RoleUserByChatDto(chatUserByRequester.Role) : null
		};
			
		return new Result<ChatDto>(chatDto);
	}
}