using Messenger.Domain.Constants;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Services.Configuration;

public class ChatUserEntityConfiguration : IEntityTypeConfiguration<ChatUserEntity>
{
	public void Configure(EntityTypeBuilder<ChatUserEntity> builder)
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
		
		var dotnetChatKaminome = new ChatUserEntity(
			SeedDataConstants.KaminomeUserId, 
			SeedDataConstants.DotnetChatId, 
			canSendMedia: true,
			muteDateOfExpire: null);
		
		var dotnetChatAlice = new ChatUserEntity(
			SeedDataConstants.AliceUserId, 
			SeedDataConstants.DotnetChatId, 
			canSendMedia: true,
			muteDateOfExpire: null);
		
		var dotnetChatBob = new ChatUserEntity(
			SeedDataConstants.BobUserId, 
			SeedDataConstants.DotnetChatId, 
			canSendMedia: true,
			muteDateOfExpire: null);
		
		var dotnetChatAlex = new ChatUserEntity(
			SeedDataConstants.AlexUserId, 
			SeedDataConstants.DotnetChatId, 
			canSendMedia: true,
			muteDateOfExpire: null);
		
		var dotnetFloodChatKaminome = new ChatUserEntity(
			SeedDataConstants.AlexUserId, 
			SeedDataConstants.DotnetFloodChatId, 
			canSendMedia: true,
			muteDateOfExpire: null);
		
		var dotnetFloodChatAlice = new ChatUserEntity(
			SeedDataConstants.AliceUserId, 
			SeedDataConstants.DotnetFloodChatId, 
			canSendMedia: true,
			muteDateOfExpire: null);
		
		var dotnetFloodChatBob = new ChatUserEntity(
			SeedDataConstants.BobUserId, 
			SeedDataConstants.DotnetFloodChatId, 
			canSendMedia: true,
			muteDateOfExpire: null);

		var dialogKamimomeAliceForKaminome = new ChatUserEntity(
			SeedDataConstants.KaminomeUserId, 
			SeedDataConstants.DialogKaminomeAliceChatId, 
			canSendMedia: true,
			muteDateOfExpire: null);
		
		var dialogKamimomeAliceForAlice = new ChatUserEntity(
			SeedDataConstants.AliceUserId, 
			SeedDataConstants.DialogKaminomeAliceChatId, 
			canSendMedia: true,
			muteDateOfExpire: null);
		
		builder.HasData(dotnetChatKaminome, dotnetChatAlice, dotnetChatBob, dotnetChatAlex,
			dotnetFloodChatKaminome, dotnetFloodChatAlice, dotnetFloodChatBob,
			dialogKamimomeAliceForAlice, dialogKamimomeAliceForKaminome);
	}
}