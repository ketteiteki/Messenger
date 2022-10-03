using FluentValidation;

namespace Messenger.Domain.Entities.Validation;

public class ChatValidator : AbstractValidator<Chat>
{
	public ChatValidator()
	{
		RuleFor(x => x.Name).NotEmpty().Length(1, 20);
		RuleFor(x => x.Title).Length(1, 20);
	}
}