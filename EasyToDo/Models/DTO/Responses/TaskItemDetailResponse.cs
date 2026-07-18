using EasyToDo.Models.Enums;

namespace EasyToDo.Models.DTO.Responses
{
    public record TaskItemDetailResponse(string Id, string? ParentTaskId, string Title, string? Description, TaskItemStatus Status, TaskItemPriority Priority, int Progress, DateTime? StartAt, DateTime? DueAt, DateTime? CompletedAt);
}
