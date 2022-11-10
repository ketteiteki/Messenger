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
		var chatUserByRequester = await _context.ChatUsers
			.Include(c => c.Chat)
			.Include(c => c.Role)
			.FirstOrDefaultAsync(c => c.UserId == request.RequesterId && 
			                          c.ChatId == request.ChatId &&
			                          c.Chat.Type != ChatType.Dialog, cancellationToken);

		if (chatUserByRequester == null)
		{
			return new Result<ChatDto>(new DbEntityNotFoundError("No requester in the chat"));
		}
		
		if (chatUserByRequester.Role is { CanChangeChatData: true } || 
		    chatUserByRequester.Chat.OwnerId == request.RequesterId)
		{
			if (chatUserByRequester.Chat.AvatarLink != null)
			{
				_fileService.DeleteFile(Path.Combine(
					BaseDirService.GetPathWwwRoot(),
					chatUserByRequester.Chat.AvatarLink.Split("/")[^1]));
				
				chatUserByRequester.Chat.AvatarLink = null;
			}
			
			if (request.AvatarFile != null)
			{
				var avatarLink = await _fileService.CreateFileAsync(BaseDirService.GetPathWwwRoot(), request.AvatarFile,
					_configuration[AppSettingConstants.MessengerDomainName]);
				chatUserByRequester.Chat.AvatarLink = avatarLink;
			}
			
			_context.Chats.Update(chatUserByRequester.Chat);
			await _context.SaveChangesAsync(cancellationToken);
			
			return new Result<ChatDto>(
				new ChatDto
				{
					Id = chatUserByRequester.ChatId,
					Name = chatUserByRequester.Chat.Name,
					Title = chatUserByRequester.Chat.Title,
					Type = chatUserByRequester.Chat.Type,
					AvatarLink = chatUserByRequester.Chat.AvatarLink,
					IsOwner = chatUserByRequester.Chat.OwnerId == request.RequesterId,
					IsMember = true,
					MuteDateOfExpire = chatUserByRequester.MuteDateOfExpire,
					BanDateOfExpire = null,
					RoleUser = chatUserByRequester.Role != null ? new RoleUserByChatDto(chatUserByRequester.Role) : null
				});
		}
		
		return new Result<ChatDto>(new ForbiddenError("It is forbidden to update someone else's chat"));
	}
}