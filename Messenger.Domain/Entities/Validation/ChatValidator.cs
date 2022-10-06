using FluentValidation;

namespace Messenger.Domain.Entities.Validation;

public class ChatValidator : AbstractValidator<Chat>
{
	public ChatValidator()
	{
		RuleFor(x => x.Name).MaximumLength(20);
		RuleFor(x => x.Title).MaximumLength(20);
	}
}