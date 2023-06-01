using Messenger.BusinessLogic.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Extensions;

public static class ResultExtension
{
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        return result.Error switch
        {
            BadRequestError badRequestError => 
                new ObjectResult(new { badRequestError.Message }) { StatusCode = 400 },
            AuthenticationError authenticationError => 
                new ObjectResult(new { authenticationError.Message }) { StatusCode = 401 },
            DbEntityExistsError dbEntityExistsError => 
                new ObjectResult(new { dbEntityExistsError.Message }) { StatusCode = 409 },
            DbEntityNotFoundError dbEntityNotFoundError => 
                new ObjectResult(new { dbEntityNotFoundError.Message }) { StatusCode = 404 },
            ForbiddenError forbiddenError =>
                new ObjectResult(new { forbiddenError.Message }) {StatusCode = 403},
			
            _ => new ObjectResult(result.Value) { StatusCode = 200 }
        };
    }
}