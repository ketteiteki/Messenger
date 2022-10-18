using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Infrastructure.Configuration;

public class MessengerCompositionRoot
{
	public static IServiceProvider Provider { get; set; }

	public static void SetProvider(IServiceProvider serviceProvider)
	{
		Provider = serviceProvider;
	}

	public static IServiceScope CreateScope()
	{
		return Provider.CreateScope();
	}
}