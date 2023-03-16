using Messenger.Domain.Constants;
using Messenger.Domain.Entities;
using Messenger.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Services.Configuration;

public class ChatEntityConfiguration : IEntityTypeConfiguration<ChatEntity>
{
	public void Configure(EntityTypeBuilder<ChatEntity> builder)
	{
		builder
			.HasOne(c => c.LastMessage)
			.WithOne(m => m.LastMessageByChat)
			.HasForeignKey<ChatEntity>(c => c.LastMessageId)
			.OnDelete(DeleteBehavior.SetNull);
		
		builder
			.HasMany(c => c.Messages)
			.WithOne(m => m.Chat)
			.HasForeignKey(m => m.ChatId)
			.OnDelete(DeleteBehavior.Cascade);
		
		builder
			.HasOne(c => c.Owner)
			.WithMany(u => u.Chats)
			.HasForeignKey(m => m.OwnerId)
			.OnDelete(DeleteBehavior.SetNull);
		
		var dotnetChat = new ChatEntity(
			title: "DotNetRuChat",
			name: "DotNetRuChat",
			type: ChatType.Conversation,
			ownerId: SeedDataConstants.KaminomeUserId,
			avatarLink: null,
			lastMessageId: null)
		{
			Id = SeedDataConstants.DotnetChatId
		};
		
		var dotnetFloodChat = new ChatEntity(
			title: ".NET Talks",
			name: "dotnettalks",
			type: ChatType.Conversation,
			ownerId: SeedDataConstants.AliceUserId,
			avatarLink: null,
			lastMessageId: null)
		{
			Id = SeedDataConstants.DotnetFloodChatId
		};
		
		var dialogKaminomeAliceChat = new ChatEntity(
			name: null,
			title: null,
			type: ChatType.Dialog,
			ownerId: null,
			avatarLink: null,
			lastMessageId: null)
		{
			Id = SeedDataConstants.DialogKaminomeAliceChatId
		};

		builder.HasData(dotnetChat, dotnetFloodChat, dialogKaminomeAliceChat);
	}
}