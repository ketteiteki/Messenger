using FluentValidation;

namespace Messenger.Domain.Entities.Validation;

public class RoleUserByChatEntityValidator : AbstractValidator<RoleUserByChatEntity>
{
	public RoleUserByChatEntityValidator()
	{
		RuleFor(x => x.UserId).NotNull();
		RuleFor(x => x.ChatId).NotNull();
		RuleFor(x => x.RoleTitle).NotEmpty().Length(1, 15);
		RuleFor(x => x.RoleColor).NotNull();
	}
}