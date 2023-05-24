using FluentValidation;
using Messenger.BusinessLogic.ApiCommands.Profiles;

namespace Messenger.BusinessLogic.Pipelines;

public class UpdateProfileDataCommandValidator : AbstractValidator<UpdateProfileDataCommand>
{
    public UpdateProfileDataCommandValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .Must(title => title.All(char.IsLetterOrDigit))
            .WithMessage("DisplayName must only contain letters or numbers.")
            .Length(1, 20);
        
        RuleFor(x => x.Nickname)
            .NotEmpty()
            .Must(title => title.All(char.IsLetterOrDigit))
            .WithMessage("Nickname must only contain letters or numbers.")
            .Length(4, 20);
        
        RuleFor(x => x.Bio).MaximumLength(70);
    }
}