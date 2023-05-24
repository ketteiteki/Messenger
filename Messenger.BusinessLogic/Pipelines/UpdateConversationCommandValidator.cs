using FluentValidation;
using Messenger.BusinessLogic.ApiCommands.Chats;

namespace Messenger.BusinessLogic.Pipelines;

public class UpdateConversationCommandValidator : AbstractValidator<UpdateChatDataCommand>
{
    public UpdateConversationCommandValidator()
    {
        RuleFor(x => x.Name)
            .Must(title => title.All(char.IsLetterOrDigit))
            .WithMessage("Name must only contain letters or numbers.")
            .Length(4, 20);
        
        RuleFor(x => x.Title)
            .Must(title => title.All(char.IsLetterOrDigit))
            .WithMessage("Title must only contain letters or numbers.")
            .Length(4, 20);
    }
}