using FluentValidation;

namespace Messenger.Domain.Entities.Validation;

public class AttachmentEntityValidator : AbstractValidator<AttachmentEntity>
{
	public AttachmentEntityValidator()
	{
		RuleFor(x => x.Link).NotEmpty();
		RuleFor(x => x.Name).NotEmpty();
		RuleFor(x => x.Size).NotNull();
		RuleFor(x => x.MessageId).NotNull();
	}
}	