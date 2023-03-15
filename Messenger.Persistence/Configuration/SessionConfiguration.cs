using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Services.Configuration;

public class SessionConfiguration : IEntityTypeConfiguration<SessionEntity>
{
    public void Configure(EntityTypeBuilder<SessionEntity> builder)
    {
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.Ip).IsRequired();
        builder.Property(x => x.UserAgent).IsRequired();
        builder.Property(x => x.ExpiresAt).IsRequired();
        builder.Property(x => x.RefreshToken).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
    }
}