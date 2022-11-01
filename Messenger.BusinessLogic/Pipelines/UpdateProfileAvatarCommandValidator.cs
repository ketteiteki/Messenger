using FluentValidation;
using Messenger.BusinessLogic.ApiCommands.Profiles;

namespace Messenger.BusinessLogic.Pipelines;

public class UpdateProfileAvatarCommandValidator : AbstractValidator<UpdateProfileAvatarCommand>
{
    public UpdateProfileAvatarCommandValidator()
    {
        RuleFor(x => x.AvatarFile)
            .NotNull()
            .SetValidator(new ImageValidator());
    }
}