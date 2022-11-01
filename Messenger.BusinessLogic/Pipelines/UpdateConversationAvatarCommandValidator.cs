using FluentValidation;
using Messenger.BusinessLogic.ApiCommands.Conversations;

namespace Messenger.BusinessLogic.Pipelines;

public class UpdateConversationAvatarCommandValidator : AbstractValidator<UpdateConversationAvatarCommand>
{
    public UpdateConversationAvatarCommandValidator()
    {
        RuleFor(x => x.AvatarFile)
            .NotNull()
            .SetValidator(new ImageValidator());
    }
}