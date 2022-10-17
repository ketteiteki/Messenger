using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public record BanUserInConversationCommand(
		Guid RequesterId,
		Guid ChatId,
		Guid UserId,
		DateTime BanDateOfExpire)
	: IRequest<Result<UserDto>>;