using FluentValidation;

namespace Messenger.Domain.Entities.Validation;

public class ChatEntityValidator : AbstractValidator<ChatEntity>
{
	public ChatEntityValidator()
	{
		RuleFor(x => x.Id).NotEmpty();
		RuleFor(x => x.Name).MaximumLength(20);
		RuleFor(x => x.Title).MaximumLength(20);
		RuleFor(x => x.Type).NotNull();
	}
}