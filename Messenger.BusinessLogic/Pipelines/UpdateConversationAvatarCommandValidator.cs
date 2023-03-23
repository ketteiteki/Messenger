using FluentValidation;
using Messenger.BusinessLogic.ApiCommands.Chats;

namespace Messenger.BusinessLogic.Pipelines;

public class UpdateConversationAvatarCommandValidator : AbstractValidator<UpdateChatAvatarCommand>
{
    public UpdateConversationAvatarCommandValidator()
    {
        RuleFor(x => x.AvatarFile)
            .NotNull()
            .SetValidator(new ImageValidator());
    }
}