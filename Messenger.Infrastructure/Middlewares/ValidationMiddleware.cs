using FluentValidation;
using Messenger.BusinessLogic.Responses;
using Microsoft.AspNetCore.Http;

namespace Messenger.Infrastructure.Middlewares;

public class ValidationMiddleware
{
    private readonly RequestDelegate _next;
        
    public ValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next.Invoke(httpContext);
        }
        catch (ValidationException e)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsJsonAsync(new BadRequestError(string.Join("; ", e.Errors)));
        }
    }
}