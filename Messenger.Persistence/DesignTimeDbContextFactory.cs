using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Messenger.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>();

        options.UseNpgsql("Server=localhost;User Id=postgres;Password=postgres;Database=MessengerDev;");

        return new DatabaseContext(options.Options);
    }
}
