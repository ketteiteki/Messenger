using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.IntegrationTests;

public static class DatabaseCleaner
{
	public static async Task Clear(this DatabaseContext databaseContext)
	{
		const string sql = "TRUNCATE TABLE \"Users\" CASCADE;" + 
		                   "TRUNCATE TABLE \"Chats\" CASCADE;" +
		                   "TRUNCATE TABLE \"Messages\" CASCADE;" +
		                   "TRUNCATE TABLE \"Attachments\" CASCADE;" +
		                   "TRUNCATE TABLE \"ChatUsers\" CASCADE;" +
		                   "TRUNCATE TABLE \"RoleUserByChats\" CASCADE;" +
		                   "TRUNCATE TABLE \"DeletedMessageByUsers\" CASCADE;" + 
		                   "TRUNCATE TABLE \"DeletedDialogByUsers\" CASCADE;" +
		                   "TRUNCATE TABLE \"Sessions\" CASCADE;" +
		                   "TRUNCATE TABLE \"BanUserByChats\" CASCADE;";

		await databaseContext.Database.ExecuteSqlRawAsync(sql);
	}
}
