namespace Messenger.WebApi;

class Program
{
	public static void Main(string[] args)
	{
		CreateWebHostBuilder(args).Build().Run();
	}

	private static IHostBuilder CreateWebHostBuilder(string[] arg) =>
		Host.CreateDefaultBuilder(arg).ConfigureWebHostDefaults(webBuilder => 
			webBuilder.UseStartup<Startup>());
}