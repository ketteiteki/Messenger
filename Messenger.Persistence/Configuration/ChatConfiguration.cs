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
		
		builder
			.HasOne(c => c.Owner)
			.WithMany(u => u.Chats)
			.HasForeignKey(m => m.OwnerId)
			.OnDelete(DeleteBehavior.SetNull);
		
		builder.Property(x => x.Id).IsRequired();
		builder.Property(x => x.Type).IsRequired();
	}
}