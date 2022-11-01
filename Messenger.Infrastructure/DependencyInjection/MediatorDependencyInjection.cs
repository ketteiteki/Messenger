using FluentValidation;
using MediatR;
using Messenger.BusinessLogic.ApiCommands.Auth;
using Messenger.BusinessLogic.ApiCommands.Profiles;
using Messenger.BusinessLogic.Pipelines;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Infrastructure.DependencyInjection;

public static class MediatorDependencyInjection
{
	public static IServiceCollection AddMediator(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
		serviceCollection.AddValidatorsFromAssembly(typeof(UpdateProfileDataCommandHandler).Assembly);
		serviceCollection.AddMediatR(typeof(RegistrationCommandHandler).Assembly);

		return serviceCollection;
	}
}