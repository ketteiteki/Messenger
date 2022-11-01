using FluentValidation;
using Messenger.BusinessLogic.ApiCommands.Messages;

namespace Messenger.BusinessLogic.Pipelines;

public class UpdateMessageCommandValidator : AbstractValidator<UpdateMessageCommand>
{
    public UpdateMessageCommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
    }
}