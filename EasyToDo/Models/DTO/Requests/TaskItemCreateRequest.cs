namespace EasyToDo.Models.DTO.Requests
{
    public record TaskItemCreateRequest(string Title, string ListId, string? Description, DateTime? StartAt, DateTime? DueAt);
}
