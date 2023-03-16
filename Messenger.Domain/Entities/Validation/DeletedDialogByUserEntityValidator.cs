using FluentValidation;

namespace Messenger.Domain.Entities.Validation;

public class DeletedDialogByUserEntityValidator : AbstractValidator<DeletedDialogByUserEntity>
{
    public DeletedDialogByUserEntityValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ChatId).NotEmpty();
    }
}