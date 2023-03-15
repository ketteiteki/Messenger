using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Services.Configuration;

public class AttachmentConfiguration : IEntityTypeConfiguration<AttachmentEntity>
{
    public void Configure(EntityTypeBuilder<AttachmentEntity> builder)
    {
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.Link).IsRequired();
        builder.Property(x => x.Size).IsRequired();
        builder.Property(x => x.MessageId).IsRequired();
    }
}