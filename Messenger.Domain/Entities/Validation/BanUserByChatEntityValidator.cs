using FluentValidation;

namespace Messenger.Domain.Entities.Validation;

public class BanUserByChatEntityValidator : AbstractValidator<BanUserByChatEntity>
{
    public BanUserByChatEntityValidator()
    {
        RuleFor(x => x.ChatId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}