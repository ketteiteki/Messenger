using Messenger.Domain.Constants;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Services.Configuration;

public class MessageEntityConfiguration : IEntityTypeConfiguration<MessageEntity>
{
	public void Configure(EntityTypeBuilder<MessageEntity> builder)
	{
		builder
			.HasOne(m => m.ReplyToMessage)
			.WithOne()
			.OnDelete(DeleteBehavior.SetNull);
		
		builder
			.HasOne(m => m.Owner)
			.WithMany(o => o.Messages)
			.HasForeignKey(o => o.OwnerId)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasMany(m => m.Attachments)
			.WithOne(a => a.Message)
			.HasForeignKey(o => o.MessageId)
			.OnDelete(DeleteBehavior.Cascade);
		
		builder.Property(x => x.Text).IsRequired();
		builder.Property(x => x.DateOfCreate).IsRequired();

		var dotnetChatMessageByBob = new MessageEntity(
			ownerId: SeedDataConstants.BobUserId,
			chatId: SeedDataConstants.DotnetChatId,
			text: "привет, какие книжки почитать?",
			replyToMessageId: null)
		{
			Id = Guid.NewGuid()
		};;
		
		var dotnetChatMessageByKaminome1 = new MessageEntity(
			ownerId: SeedDataConstants.KaminomeUserId,
			chatId: SeedDataConstants.DotnetChatId,
			text: "Книги в айтишке это как предметы в школе, созданы что б отбить у тебя желание учиться...",
			replyToMessageId: dotnetChatMessageByBob.Id)
		{
			Id = Guid.NewGuid()
		};
		
		var dotnetChatMessageByAlice = new MessageEntity(
			ownerId: SeedDataConstants.AliceUserId,
			chatId: SeedDataConstants.DotnetChatId,
			text: "ладно",
			replyToMessageId: dotnetChatMessageByKaminome1.Id);

		var dotnetChatMessageByKaminome2 = new MessageEntity(
			ownerId: SeedDataConstants.KaminomeUserId,
			chatId: SeedDataConstants.DotnetChatId,
			text: "ага",
			replyToMessageId: null);

		var dialogKaminomeAliceMessageByKaminome = new MessageEntity(
			ownerId: SeedDataConstants.KaminomeUserId,
			chatId: SeedDataConstants.DialogKaminomeAliceChatId,
			replyToMessageId: null,
			text: "привет");
		
		var dialogKaminomeAliceMessageByAlice = new MessageEntity(
			ownerId: SeedDataConstants.KaminomeUserId,
			chatId: SeedDataConstants.DialogKaminomeAliceChatId,
			replyToMessageId: null,
			text: "привет, как дела?");

		builder.HasData(dotnetChatMessageByBob, dotnetChatMessageByKaminome1, dotnetChatMessageByAlice,
			dotnetChatMessageByKaminome2, dialogKaminomeAliceMessageByKaminome, dialogKaminomeAliceMessageByAlice);
	}
}