using EasyToDo.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
namespace EasyToDo.Services
{
    public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var traceId = httpContext.TraceIdentifier;
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            logger.LogError(
                exception,
                "GlobalExceptionHandler Received An Unexpected Exception. TraceId: {TraceId}, Method: {Method}, Path: {Path}",
                traceId,
                httpContext.Request.Method,
                httpContext.Request.Path
                );
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "GlobalExceptionHandler Received An Unexpected Exception.",
                Detail =  exception.Message,
                Instance = httpContext.Request.Path,
                Extensions = { ["traceId"] = traceId }
            };
            await httpContext.Response.WriteAsJsonAsync(
                new ApiResponse<ProblemDetails>()
                {  
                    Data = problemDetails,
                    Success = false,
                    Message = $"An unexpected error occurred. Please contact support team."
                }, 
                cancellationToken: cancellationToken);
            return true;
        }
    }
}
