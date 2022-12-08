using Messenger.Domain.Constants;
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

		var dotnetChatKaminome = new ChatUser()
		{
			UserId = SeedDataConstants.KaminomeUserId,
			ChatId = SeedDataConstants.DotnetChatId
		};
		
		var dotnetChatAlice = new ChatUser()
		{
			UserId = SeedDataConstants.AliceUserId,
			ChatId = SeedDataConstants.DotnetChatId
		};
		
		var dotnetChatBob = new ChatUser()
		{
			UserId = SeedDataConstants.BobUserId,
			ChatId = SeedDataConstants.DotnetChatId
		};
		
		var dotnetChatAlex = new ChatUser()
		{
			UserId = SeedDataConstants.AlexUserId,
			ChatId = SeedDataConstants.DotnetChatId
		};
		
		var dotnetFloodChatKaminome = new ChatUser()
		{
			UserId = SeedDataConstants.KaminomeUserId,
			ChatId = SeedDataConstants.DotnetFloodChatId
		};
		
		var dotnetFloodChatAlice = new ChatUser()
		{
			UserId = SeedDataConstants.AliceUserId,
			ChatId = SeedDataConstants.DotnetFloodChatId
		};
		
		var dotnetFloodChatBob = new ChatUser()
		{
			UserId = SeedDataConstants.BobUserId,
			ChatId = SeedDataConstants.DotnetFloodChatId
		};
		
		var dialogKamimomeAliceForKaminome = new ChatUser()
		{
			UserId = SeedDataConstants.KaminomeUserId,
			ChatId = SeedDataConstants.DialogKaminomeAliceChatId
		};
		
		var dialogKamimomeAliceForAlice = new ChatUser()
		{
			UserId = SeedDataConstants.AliceUserId,
			ChatId = SeedDataConstants.DialogKaminomeAliceChatId
		};

		builder.HasData(dotnetChatKaminome, dotnetChatAlice, dotnetChatBob, dotnetChatAlex,
			dotnetFloodChatKaminome, dotnetFloodChatAlice, dotnetFloodChatBob,
			dialogKamimomeAliceForAlice, dialogKamimomeAliceForKaminome);
	}
}