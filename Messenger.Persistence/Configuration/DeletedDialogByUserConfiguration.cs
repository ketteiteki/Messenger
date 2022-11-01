using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Services.Configuration;

public class DeletedDialogByUserConfiguration : IEntityTypeConfiguration<DeletedDialogByUser>
{
	public void Configure(EntityTypeBuilder<DeletedDialogByUser> builder)
	{
		builder
			.HasKey(d => new { d.ChatId, d.UserId }); 
		
		builder
			.HasOne(p => p.Chat)
			.WithMany(c => c.DeletedDialogByUsers)
			.HasForeignKey(p => p.ChatId)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasOne(p => p.User)
			.WithMany(u => u.DeletedDialogByUsers)
			.HasForeignKey(p => p.UserId)
			.OnDelete(DeleteBehavior.Cascade);
		
		builder.Property(x => x.ChatId).IsRequired();
		builder.Property(x => x.UserId).IsRequired();
	}
}