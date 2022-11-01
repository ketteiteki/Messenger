using FluentValidation;
using Messenger.BusinessLogic.ApiCommands.Chats;

namespace Messenger.BusinessLogic.Pipelines;

public class CreateChatCommandValidator : AbstractValidator<CreateChatCommand>
{
    public CreateChatCommandValidator()
    {
        RuleFor(x => x.Name).Length(4, 20);
        RuleFor(x => x.Title).Length(4, 20);

        RuleFor(x => x.AvatarFile)
            .SetValidator(new ImageValidator());
    }
}