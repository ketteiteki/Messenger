using FluentValidation;

namespace Messenger.Domain.Entities.Validation;

public class DeletedDialogByUserEntityValidator : AbstractValidator<DeletedDialogByUserEntity>
{
    public DeletedDialogByUserEntityValidator()
    {
        RuleFor(x => x.UserId).NotNull();
        RuleFor(x => x.ChatId).NotNull();
    }
}