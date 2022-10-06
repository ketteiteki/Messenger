using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Services;

public class DatabaseContext : DbContext
{
	public DbSet<User> Users { get; set; } = null!;
	public DbSet<Chat> Chats { get; set; } = null!;
	public DbSet<Message> Messages { get; set; } = null!;
	public DbSet<Attachment> Attachments { get; set; } = null!;
	public DbSet<ChatUser> ChatUsers { get; set; } = null!;
	public DbSet<RoleUserByChat> RoleUserByChats { get; set; } = null!;
	public DbSet<DeletedMessageByUser> DeletedMessageByUsers { get; set; } = null!;
	public DbSet<DeletedDialogByUser> DeletedDialogByUsers { get; set; } = null!;
	public DbSet<BanUserByChat> BanUserByChats { get; set; } = null!;

	public DatabaseContext(DbContextOptions<DatabaseContext> dbContextOptions) : base(dbContextOptions)
	{
		base.Database.EnsureCreated();
	}
	
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		//message
		modelBuilder
			.Entity<Message>()
			.HasOne(m => m.Owner)
			.WithMany(o => o.Messages)
			.HasForeignKey(o => o.OwnerId)
			.OnDelete(DeleteBehavior.NoAction);

		modelBuilder
			.Entity<Message>()
			.HasMany(m => m.Attachments)
			.WithOne(a => a.Message)
			.HasForeignKey(o => o.MessageId)
			.OnDelete(DeleteBehavior.Cascade);
		
		//chat
		modelBuilder
			.Entity<Chat>()
			.HasOne(c => c.LastMessage)
			.WithOne(m => m.LastMessageByChat)
			.HasForeignKey<Chat>(c => c.LastMessageId)
			.OnDelete(DeleteBehavior.SetNull);
		
		modelBuilder
			.Entity<Chat>()
			.HasMany(c => c.Messages)
			.WithOne(m => m.Chat)
			.HasForeignKey(m => m.ChatId)
			.OnDelete(DeleteBehavior.Cascade);
		
		//roleUserByChat
		modelBuilder
			.Entity<RoleUserByChat>()
			.HasKey(c => new {c.ChatId, c.UserId});
		
		modelBuilder
			.Entity<RoleUserByChat>()
			.HasOne(r => r.ChatUser)
			.WithOne(c => c.Role)
			.HasForeignKey<RoleUserByChat>(c => new {c.ChatId, c.UserId});
		
		//chatUser
		modelBuilder
			.Entity<ChatUser>()
			.HasKey(c => new {c.ChatId, c.UserId});
		
		modelBuilder
			.Entity<ChatUser>()
			.HasOne(p => p.Chat)
			.WithMany(c => c.ChatUsers)
			.HasForeignKey(p => p.ChatId)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder
			.Entity<ChatUser>()
			.HasOne(p => p.User)
			.WithMany(u => u.ChatUsers)
			.HasForeignKey(p => p.UserId)
			.OnDelete(DeleteBehavior.Cascade);
		
		//BanUserByChat
		modelBuilder
			.Entity<BanUserByChat>()
			.HasKey(b => new {b.ChatId, b.UserId});

		modelBuilder
			.Entity<BanUserByChat>()
			.HasOne(b => b.Chat)
			.WithMany(c => c.BanUserByChats)
			.HasForeignKey(b => b.ChatId)
			.OnDelete(DeleteBehavior.Cascade);
		
		modelBuilder
			.Entity<BanUserByChat>()
			.HasOne(b => b.User)
			.WithMany(c => c.BanUserByChats)
			.HasForeignKey(b => b.UserId)
			.OnDelete(DeleteBehavior.Cascade);
		
		//DeletedMessageByUser
		modelBuilder.Entity<DeletedMessageByUser>()
			.HasKey(bc => new { bc.MessageId, bc.UserId }); 
		
		modelBuilder
			.Entity<DeletedMessageByUser>()
			.HasOne(p => p.Message)
			.WithMany(c => c.DeletedMessageByUsers)
			.HasForeignKey(p => p.MessageId)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder
			.Entity<DeletedMessageByUser>()
			.HasOne(p => p.User)
			.WithMany(u => u.DeletedMessageByUsers)
			.HasForeignKey(p => p.UserId)
			.OnDelete(DeleteBehavior.Cascade);
		
		//DeletedDialogByUsers
		modelBuilder.Entity<DeletedDialogByUser>()
			.HasKey(d => new { d.ChatId, d.UserId }); 
		
		modelBuilder
			.Entity<DeletedDialogByUser>()
			.HasOne(p => p.Chat)
			.WithMany(c => c.DeletedDialogByUsers)
			.HasForeignKey(p => p.ChatId)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder
			.Entity<DeletedDialogByUser>()
			.HasOne(p => p.User)
			.WithMany(u => u.DeletedDialogByUsers)
			.HasForeignKey(p => p.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}