using Messenger.BusinessLogic.Auth.Commands;

namespace Messenger.IntegrationTests.Helpers;

public static class CommandHelper
{
	//registraion
	public static RegistrationCommand Registration21thCommand()
	{
		return new RegistrationCommand
		{
			DisplayName = "21th",
			Nickname = "ketteiteki",
			Password = "1234567890"
		};
	}
}