using EasyToDo.Models.Enums;

namespace EasyToDo.Models.DTO.Requests
{
    public record TaskItemUpdateRequest(string Title, string ListId, string? Description, TaskItemStatus Status, TaskItemPriority Priority, int Progress, DateTime? StartAt, DateTime? DueAt);
}
