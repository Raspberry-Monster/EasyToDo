namespace EasyToDo.Models.DTO.Requests
{
    public record TaskItemCreateRequest(string Title, Guid? ListId, string? Description, DateTime? StartAt, DateTime? DueAt);
}
