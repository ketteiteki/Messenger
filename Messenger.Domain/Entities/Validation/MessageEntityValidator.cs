using FluentValidation;

namespace Messenger.Domain.Entities.Validation;

public class MessageEntityValidator : AbstractValidator<MessageEntity>
{
	public MessageEntityValidator()
	{
		RuleFor(x => x.Id).NotEmpty();
		RuleFor(x => x.Text).NotEmpty();
		RuleFor(x => x.ChatId).NotEmpty();
	}
}