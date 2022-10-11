using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Services.Configuration;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
	public void Configure(EntityTypeBuilder<Message> builder)
	{
		builder
			.HasOne(m => m.Owner)
			.WithMany(o => o.Messages)
			.HasForeignKey(o => o.OwnerId)
			.OnDelete(DeleteBehavior.NoAction);

		builder
			.HasMany(m => m.Attachments)
			.WithOne(a => a.Message)
			.HasForeignKey(o => o.MessageId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}