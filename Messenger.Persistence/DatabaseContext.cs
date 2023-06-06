using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Messenger.Persistence;

public class DatabaseContext : DbContext
{
	public DbSet<UserEntity> Users { get; set; }
	public DbSet<ChatEntity> Chats { get; set; }
	public DbSet<MessageEntity> Messages { get; set; }
	public DbSet<AttachmentEntity> Attachments { get; set; }
	public DbSet<ChatUserEntity> ChatUsers { get; set; }
	public DbSet<RoleUserByChatEntity> RoleUserByChats { get; set; }
	public DbSet<DeletedMessageByUserEntity> DeletedMessageByUsers { get; set; }
	public DbSet<DeletedDialogByUserEntity> DeletedDialogByUsers { get; set; }
	public DbSet<BanUserByChatEntity> BanUserByChats { get; set; }
	public DbSet<UserSessionEntity> UserSessions { get; set; }

	public DatabaseContext(DbContextOptions<DatabaseContext> dbContextOptions) : base(dbContextOptions)
	{
	}
	
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
	}
}