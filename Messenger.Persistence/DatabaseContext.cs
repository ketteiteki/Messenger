using System.Reflection;
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
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
	}
}