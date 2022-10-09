using Messenger.BusinessLogic.ApiCommands.Auth;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Microsoft.AspNetCore.Http;

namespace Messenger.IntegrationTests.Helpers;

public static class CommandHelper
{
	//registraion
	public static RegistrationCommand Registration21thCommand()
	{
		return new RegistrationCommand("21th", "ketteiteki", "1234567890");
	}
	
	public static RegistrationCommand RegistrationAliceCommand()
	{
		return new RegistrationCommand("alice", "alice123", "3254321f");
	}
	
	public static RegistrationCommand RegistrationBobCommand()
	{
		return new RegistrationCommand("bob", "bob123", "gbv43rf");
	}
	
	public static RegistrationCommand RegistrationAlexCommand()
	{
		return new RegistrationCommand("alex", "alex123", "765cs3131");
	}
	//conversation
	public static CreateConversationCommand CreateConversationCommand(Guid requesterId, string title, string name,
		IFormFile? avatarFile)
	{
		return new CreateConversationCommand(requesterId, name, title, avatarFile);
	}
	
}