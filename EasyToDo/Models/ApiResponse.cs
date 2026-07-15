namespace EasyToDo.Models
{
    public sealed class ApiResponse<T> where T : class, new()
    {
        public T? Data { get; init; }
        public bool Success { get; init; }
        public string? Message { get; init; }
    }
}
