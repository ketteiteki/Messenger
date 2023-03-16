using FluentValidation;

namespace Messenger.Domain.Entities.Validation;

public class DeletedMessageByUserEntityValidator : AbstractValidator<DeletedMessageByUserEntity>
{
    public DeletedMessageByUserEntityValidator()
    {
        RuleFor(x => x.MessageId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}