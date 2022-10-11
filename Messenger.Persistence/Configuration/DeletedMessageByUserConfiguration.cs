using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Services.Configuration;

public class DeletedMessageByUserConfiguration : IEntityTypeConfiguration<DeletedMessageByUser>
{
	public void Configure(EntityTypeBuilder<DeletedMessageByUser> builder)
	{
		builder
			.HasKey(bc => new { bc.MessageId, bc.UserId }); 
		
		builder
			.HasOne(p => p.Message)
			.WithMany(c => c.DeletedMessageByUsers)
			.HasForeignKey(p => p.MessageId)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasOne(p => p.User)
			.WithMany(u => u.DeletedMessageByUsers)
			.HasForeignKey(p => p.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}