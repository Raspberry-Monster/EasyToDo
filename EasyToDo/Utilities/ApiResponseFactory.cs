using EasyToDo.Models;

namespace EasyToDo.Utilities
{
    public static class ApiResponseFactory
    {
        public static ApiResponse<T> Success<T>(T? data, string message) where T : class => new() { Data = data, Message = message, Success = true };

        public static ApiResponse<T> Failure<T>(string message) where T : class => new() { Message = message, Success = false };
    }
}
