namespace EasyToDo.Models.DTO.Requests
{
    public record SubTaskCreateRequest(string Title, string? Description, DateTime? StartAt, DateTime? DueAt);
}
