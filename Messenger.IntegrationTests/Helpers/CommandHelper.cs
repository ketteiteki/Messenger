using Messenger.BusinessLogic.ApiCommands.Auth;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Microsoft.AspNetCore.Http;

namespace Messenger.IntegrationTests.Helpers;

public static class CommandHelper
{
	//registration
	public static RegistrationCommand Registration21ThCommand()
	{
		return new RegistrationCommand(
			DisplayName: "21th",
			Nickname: "ketteiteki",
			Password: "1234567890");
	}
	
	public static RegistrationCommand RegistrationAliceCommand()
	{
		return new RegistrationCommand(
			DisplayName: "alice", 
			Nickname: "alice123", 
			Password: "3254321f");
	}
	
	public static RegistrationCommand RegistrationBobCommand()
	{
		return new RegistrationCommand(
			DisplayName: "bob",
			Nickname: "bob123",
			Password: "gbv43rf");
	}
	
	public static RegistrationCommand RegistrationAlexCommand()
	{
		return new RegistrationCommand(
			DisplayName: "alex", 
			Nickname: "alex123",
			Password: "765cs3131");
	}
	//conversation
	public static CreateConversationCommand CreateConversationCommand(Guid requesterId, string title, string name,
		IFormFile? avatarFile)
	{
		return new CreateConversationCommand(
			RequesterId: requesterId,
			Name: name, 
			Title: title,
			AvatarFile: avatarFile);
	}
	
}