using FluentValidation;

namespace Messenger.Domain.Entities.Validation;

public class AttachmentValidator : AbstractValidator<Attachment>
{
	public AttachmentValidator()
	{
		RuleFor(x => x.Link).NotEmpty();
		RuleFor(x => x.Name).NotEmpty();
		RuleFor(x => x.Size).NotNull();
		RuleFor(x => x.MessageId).NotNull();
	}
}	