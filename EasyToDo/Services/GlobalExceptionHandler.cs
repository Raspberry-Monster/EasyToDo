using EasyToDo.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
namespace EasyToDo.Services
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(
                new ApiResponse<object>()
                {
                    Success = false,
                    Message = $"An unexpected error occurred. Please contact support team. Error: {exception.Message}"
                }, 
                cancellationToken: cancellationToken);
            return true;
        }
    }
}
