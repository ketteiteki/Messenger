using FluentValidation;

namespace Messenger.Domain.Entities.Validation;

public class ChatUserEntityValidator : AbstractValidator<ChatUserEntity>
{
    public ChatUserEntityValidator()
    {
        RuleFor(x => x.ChatId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}