using Messenger.Domain.Constants;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Services.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .HasMany(u => u.Sessions)
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.DisplayName).IsRequired();
        builder.Property(x => x.Nickname).IsRequired();

        var kaminome = new User(
            displayName: "kami no me",
            nickname: "kaminome",
            bio: "the best account",
            avatarLink: null,
            passwordHash: "gzF/n+F8YPd/IvNrALE/XtGhhoJhtRs+PP3eco6JYzB36pVy2TGyj/4+68GXGws4EiIjSAkPKutdJuj6tb0d7A==",
            passwordSalt: "fh1cbqngj2gJnAoolbwi6e6tPVGwUnrLVsCX1l7UbD+Nxz72Y8F4ucWNaBa0kLopPAyFWHesvCfZX7OSlqG3ZVAjYTUIa+YCV3TXwgNnQARH0KptctnRHczMzlk5D0bmHra29Zc3rGkWpsxtVGhuayb/FIUGPG92Md0G8d6v2GI="
        )
        {
            Id = SeedDataConstants.KaminomeUserId
        };
        
        var alice = new User(
            displayName: "alice1",
            nickname: "alice1234",
            bio: "cool status",
            avatarLink: null,
            passwordHash: "gzF/n+F8YPd/IvNrALE/XtGhhoJhtRs+PP3eco6JYzB36pVy2TGyj/4+68GXGws4EiIjSAkPKutdJuj6tb0d7A==",
            passwordSalt: "fh1cbqngj2gJnAoolbwi6e6tPVGwUnrLVsCX1l7UbD+Nxz72Y8F4ucWNaBa0kLopPAyFWHesvCfZX7OSlqG3ZVAjYTUIa+YCV3TXwgNnQARH0KptctnRHczMzlk5D0bmHra29Zc3rGkWpsxtVGhuayb/FIUGPG92Md0G8d6v2GI="
        )
        {
            Id = SeedDataConstants.AliceUserId
        };
        
        var bob = new User(
            displayName: "bob1",
            nickname: "bob1234",
            bio: "I'm Bob",
            avatarLink: null,
            passwordHash: "gzF/n+F8YPd/IvNrALE/XtGhhoJhtRs+PP3eco6JYzB36pVy2TGyj/4+68GXGws4EiIjSAkPKutdJuj6tb0d7A==",
            passwordSalt: "fh1cbqngj2gJnAoolbwi6e6tPVGwUnrLVsCX1l7UbD+Nxz72Y8F4ucWNaBa0kLopPAyFWHesvCfZX7OSlqG3ZVAjYTUIa+YCV3TXwgNnQARH0KptctnRHczMzlk5D0bmHra29Zc3rGkWpsxtVGhuayb/FIUGPG92Md0G8d6v2GI="
        )
        {
            Id = SeedDataConstants.BobUserId
        };
        
        var alex = new User(
            displayName: "alex1",
            nickname: "alex1234",
            bio: "why alex?",
            avatarLink: null,
            passwordHash: "gzF/n+F8YPd/IvNrALE/XtGhhoJhtRs+PP3eco6JYzB36pVy2TGyj/4+68GXGws4EiIjSAkPKutdJuj6tb0d7A==",
            passwordSalt: "fh1cbqngj2gJnAoolbwi6e6tPVGwUnrLVsCX1l7UbD+Nxz72Y8F4ucWNaBa0kLopPAyFWHesvCfZX7OSlqG3ZVAjYTUIa+YCV3TXwgNnQARH0KptctnRHczMzlk5D0bmHra29Zc3rGkWpsxtVGhuayb/FIUGPG92Md0G8d6v2GI="
        )
        {
            Id = SeedDataConstants.AlexUserId
        };
        
        builder.HasData(kaminome, alice, bob, alex);
    }
}