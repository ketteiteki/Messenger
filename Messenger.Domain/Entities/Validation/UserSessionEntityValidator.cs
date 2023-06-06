using FluentValidation;

namespace Messenger.Domain.Entities.Validation;

public class UserSessionEntityValidator : AbstractValidator<UserSessionEntity>
{
    public UserSessionEntityValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.CreatedAt).NotNull();
        RuleFor(x => x.ExpiresAt).NotNull();
        RuleFor(x => x.UpdatedAt).NotNull();
        RuleFor(x => x.Value).NotNull();
        RuleFor(x => x.UserId).NotEmpty();
    }
}