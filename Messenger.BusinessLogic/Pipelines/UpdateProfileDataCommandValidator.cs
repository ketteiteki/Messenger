using FluentValidation;
using Messenger.BusinessLogic.ApiCommands.Profiles;

namespace Messenger.BusinessLogic.Pipelines;

public class UpdateProfileDataCommandValidator : AbstractValidator<UpdateProfileDataCommand>
{
    public UpdateProfileDataCommandValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty().Length(1, 20);
        RuleFor(x => x.Nickname).NotEmpty().Length(4, 20);
        RuleFor(x => x.Bio).MaximumLength(70);
    }
}