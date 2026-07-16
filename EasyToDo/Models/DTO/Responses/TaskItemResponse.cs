using EasyToDo.Models.Enums;

namespace EasyToDo.Models.DTO.Responses
{
    public record TaskItemResponse(string Title, TaskItemStatus Status, TaskItemPriority Priority, int Progress, DateTime StartAt, DateTime DueAt);
}
