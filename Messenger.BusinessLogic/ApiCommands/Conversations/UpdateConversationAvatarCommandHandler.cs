using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Constants;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public class UpdateConversationAvatarCommandHandler : IRequestHandler<UpdateConversationAvatarCommand, Result<ChatDto>>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;
	
	public UpdateConversationAvatarCommandHandler(DatabaseContext context, IFileService fileService)
	{
		_context = context;
		_fileService = fileService;
	}
	
	public async Task<Result<ChatDto>> Handle(UpdateConversationAvatarCommand request, CancellationToken cancellationToken)
	{
		var chatUserByRequester = await _context.ChatUsers
			.Include(c => c.Chat)
			.Include(c => c.Role)
			.FirstOrDefaultAsync(r => r.UserId == request.RequestorId && r.ChatId == request.ChatId, cancellationToken);

		if (chatUserByRequester == null)
			return new Result<ChatDto>(new DbEntityNotFoundError("No requestor in the chat"));
		
		if (chatUserByRequester.Role is { CanChangeChatData: true } || 
		    chatUserByRequester.Chat.OwnerId == request.RequestorId)
		{
			if (chatUserByRequester.Chat.AvatarLink != null)
			{
				_fileService.DeleteFile(Path.Combine(
					EnvironmentConstants.GetPathWWWRoot(),
					chatUserByRequester.Chat.AvatarLink.Split("/")[^1]));
				chatUserByRequester.Chat.AvatarLink = null;
			}
			
			if (request.AvatarFile != null)
			{
				var avatarLink = await _fileService.CreateFileAsync(EnvironmentConstants.GetPathWWWRoot(), request.AvatarFile);
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
					IsOwner = chatUserByRequester.Chat.OwnerId == request.RequestorId,
					IsMember = true,
					MuteDateOfExpire = chatUserByRequester.MuteDateOfExpire,
					BanDateOfExpire = null,
					RoleUser = chatUserByRequester.Role != null ? new RoleUserByChatDto(chatUserByRequester.Role) : null
				});
		}
		
		return new Result<ChatDto>(new ForbiddenError("It is forbidden to update someone else's conversation"));
	}
}