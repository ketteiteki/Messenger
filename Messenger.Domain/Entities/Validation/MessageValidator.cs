using FluentValidation;

namespace Messenger.Domain.Entities.Validation;

public class MessageValidator : AbstractValidator<Message>
{
	public MessageValidator()
	{
		RuleFor(x => x.Text).NotEmpty();
		RuleFor(x => x.OwnerId).NotNull();
	}
}