namespace EasyToDo.Models
{
    public sealed class ApiResponse<T> where T : class
    {
        public T? Data { get; init; }
        public required bool Success { get; init; }
        public string? Message { get; init; }
    }
}
