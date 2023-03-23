using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Services.Configuration;

public class AttachmentEntityConfiguration : IEntityTypeConfiguration<AttachmentEntity>
{
    public void Configure(EntityTypeBuilder<AttachmentEntity> builder)
    {
        builder.Property(x => x.FileName).IsRequired();
    }
}