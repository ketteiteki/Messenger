using FluentValidation;
using Messenger.BusinessLogic.ApiCommands.Messages;

namespace Messenger.BusinessLogic.Pipelines;

public class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
{
    public CreateMessageCommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
        RuleForEach(x => x.Files).SetValidator(new ImageValidator());
    }
}