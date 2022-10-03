using Microsoft.AspNetCore.Http;

namespace Messenger.BusinessLogic.Middlewares;

public class ExceptionMiddleware
{
	private readonly RequestDelegate _next;

	public ExceptionMiddleware(RequestDelegate next)
	{
		_next = next;
	}
	
	public async Task InvokeAsync(HttpContext context)
	{
		// try
		// {
			await _next.Invoke(context);
		// }
		// catch (Exception e)
		// {
		// 	switch (e)
		// 	{
		// 		case BadRequestException:
		// 			context.Response.StatusCode = 400;
		// 			await context.Response.WriteAsJsonAsync("");
		// 			break;
		// 	
		// 		case AuthenticationException:
		// 			context.Response.StatusCode = 401;
		// 			await context.Response.WriteAsJsonAsync("");
		// 			break;
		// 	
		// 		case ForbiddenException:
		// 			context.Response.StatusCode = 403;
		// 			await context.Response.WriteAsJsonAsync("");
		// 			break;
		// 	
		// 		case DbEntityNotFoundException:
		// 			context.Response.StatusCode = 404;
		// 			await context.Response.WriteAsJsonAsync("");
		// 			break;
		// 	
		// 		case DbEntityExistsException:
		// 			context.Response.StatusCode = 409;
		// 			await context.Response.WriteAsJsonAsync("");
		// 			break;
		// 	
		// 		case InvalidOperationException:
		// 			context.Response.StatusCode = 409;
		// 			await context.Response.WriteAsJsonAsync("");
		// 			break;
		// 	
		// 		default:
		// 			context.Response.StatusCode = 500;
		// 			await context.Response.WriteAsJsonAsync("");
		// 			break;
		// 	}
		// }
	}
}