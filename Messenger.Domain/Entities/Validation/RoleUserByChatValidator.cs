using FluentValidation;

namespace Messenger.Domain.Entities.Validation;

public class RoleUserByChatValidator : AbstractValidator<RoleUserByChat>
{
	public RoleUserByChatValidator()
	{
		RuleFor(x => x.RoleTitle).NotEmpty().Length(1, 15);
	}
}