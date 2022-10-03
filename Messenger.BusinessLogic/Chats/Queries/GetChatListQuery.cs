using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Chats.Queries;

public class GetChatListQuery : IRequest<List<ChatDto>>
{
	public Guid RequesterId { get; set; }
}