using FluentValidation;
using Messenger.BusinessLogic.ApiCommands.Conversations;

namespace Messenger.BusinessLogic.Pipelines;

public class UpdateConversationCommandValidator : AbstractValidator<UpdateConversationCommand>
{
    public UpdateConversationCommandValidator()
    {
        RuleFor(x => x.Name).Length(4, 20);
        RuleFor(x => x.Title).Length(4, 20);
    }
}