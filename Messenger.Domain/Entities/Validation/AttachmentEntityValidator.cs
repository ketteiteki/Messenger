using FluentValidation;

namespace Messenger.Domain.Entities.Validation;

public class AttachmentEntityValidator : AbstractValidator<AttachmentEntity>
{
	public AttachmentEntityValidator()
	{
		RuleFor(x => x.Id).NotEmpty();
		RuleFor(x => x.FileName).NotEmpty();
		RuleFor(x => x.Size).NotNull();
		RuleFor(x => x.MessageId).NotEmpty();
	}
}	