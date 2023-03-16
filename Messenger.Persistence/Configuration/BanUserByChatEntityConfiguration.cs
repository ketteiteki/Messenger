using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Services.Configuration;

public class BanUserByChatEntityConfiguration : IEntityTypeConfiguration<BanUserByChatEntity>
{
	public void Configure(EntityTypeBuilder<BanUserByChatEntity> builder)
	{
		builder
			.HasKey(b => new {b.ChatId, b.UserId});

		builder
			.HasOne(b => b.Chat)
			.WithMany(c => c.BanUserByChats)
			.HasForeignKey(b => b.ChatId)
			.OnDelete(DeleteBehavior.Cascade);
		
		builder
			.HasOne(b => b.User)
			.WithMany(c => c.BanUserByChats)
			.HasForeignKey(b => b.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}