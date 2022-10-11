using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Services.Configuration;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
	public void Configure(EntityTypeBuilder<Chat> builder)
	{
		builder
			.HasOne(c => c.LastMessage)
			.WithOne(m => m.LastMessageByChat)
			.HasForeignKey<Chat>(c => c.LastMessageId)
			.OnDelete(DeleteBehavior.SetNull);
		
		builder
			.HasMany(c => c.Messages)
			.WithOne(m => m.Chat)
			.HasForeignKey(m => m.ChatId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}