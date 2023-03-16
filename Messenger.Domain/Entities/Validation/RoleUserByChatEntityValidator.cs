using FluentValidation;

namespace Messenger.Domain.Entities.Validation;

public class RoleUserByChatEntityValidator : AbstractValidator<RoleUserByChatEntity>
{
	public RoleUserByChatEntityValidator()
	{
		RuleFor(x => x.UserId).NotEmpty();
		RuleFor(x => x.ChatId).NotEmpty();
		RuleFor(x => x.RoleTitle).NotEmpty().Length(1, 15);
		RuleFor(x => x.RoleColor).NotNull();
	}
}