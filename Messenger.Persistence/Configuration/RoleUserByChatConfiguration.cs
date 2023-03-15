using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Services.Configuration;

public class RoleUserByChatConfiguration : IEntityTypeConfiguration<RoleUserByChatEntity>
{
	public void Configure(EntityTypeBuilder<RoleUserByChatEntity> builder)
	{
		builder
			.HasKey(c => new {c.ChatId, c.UserId});
        		
		builder
			.HasOne(r => r.ChatUser)
			.WithOne(c => c.Role)
			.HasForeignKey<RoleUserByChatEntity>(c => new {c.ChatId, c.UserId})
			.OnDelete(DeleteBehavior.Cascade);
		
		builder.Property(x => x.ChatId).IsRequired();
		builder.Property(x => x.UserId).IsRequired();
	}
}