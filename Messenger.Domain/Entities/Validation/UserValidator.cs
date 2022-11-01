using FluentValidation;

namespace Messenger.Domain.Entities.Validation;

public class UserValidator : AbstractValidator<User>
{
	public UserValidator()
	{
		RuleFor(x => x.DisplayName).NotEmpty().Length(1, 20);
		RuleFor(x => x.Nickname).NotEmpty().Length(4, 20);
		RuleFor(x => x.Bio).MaximumLength(70);
		RuleFor(x => x.PasswordHash).NotEmpty();
		RuleFor(x => x.PasswordSalt).NotEmpty();
	}
}