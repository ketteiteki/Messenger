using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Services.Configuration;

public class SessionEntityConfiguration : IEntityTypeConfiguration<SessionEntity>
{
    public void Configure(EntityTypeBuilder<SessionEntity> builder)
    {
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.AccessToken).IsRequired();
        builder.Property(x => x.Ip).IsRequired();
        builder.Property(x => x.UserAgent).IsRequired();
        builder.Property(x => x.ExpiresAt).IsRequired();
        builder.Property(x => x.CreateAt).IsRequired();
        builder.Property(x => x.RefreshToken).IsRequired();
    }
}