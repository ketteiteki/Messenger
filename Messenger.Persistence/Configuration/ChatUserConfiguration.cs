using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Services.Configuration;

public class ChatUserConfiguration : IEntityTypeConfiguration<ChatUser>
{
	public void Configure(EntityTypeBuilder<ChatUser> builder)
	{
		builder
			.HasKey(c => new {c.ChatId, c.UserId});
		
		builder
			.HasOne(p => p.Chat)
			.WithMany(c => c.ChatUsers)
			.HasForeignKey(p => p.ChatId)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasOne(p => p.User)
			.WithMany(u => u.ChatUsers)
			.HasForeignKey(p => p.UserId)
			.OnDelete(DeleteBehavior.Cascade);
		
		builder.Property(x => x.ChatId).IsRequired();
		builder.Property(x => x.UserId).IsRequired();
	}
}