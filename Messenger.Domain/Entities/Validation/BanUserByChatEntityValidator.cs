using FluentValidation;

namespace Messenger.Domain.Entities.Validation;

public class BanUserByChatEntityValidator : AbstractValidator<BanUserByChatEntity>
{
    public BanUserByChatEntityValidator()
    {
        RuleFor(x => x.ChatId).NotNull();
        RuleFor(x => x.UserId).NotNull();
        RuleFor(x => x.BanDateOfExpire).NotNull();
    }
}