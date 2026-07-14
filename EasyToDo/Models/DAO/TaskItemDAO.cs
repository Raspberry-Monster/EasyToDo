using EasyToDo.Models.Enums;

namespace EasyToDo.Models.DAO
{
    public class TaskItemDAO
    {
        public Guid Id { get; set; }
        public required Guid ListId { get; set; }
        public Guid? ParentTaskId { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public TaskItemStatus Status { get; set; } = TaskItemStatus.Stopped;
        public TaskItemPriority Priority { get; set; } = TaskItemPriority.Normal;
        public int Progress { get; set; } = 0;
        public DateTime? StartAt { get; set; }
        public DateTime? DueAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public DateTime? CreatedAt { get; }
        public DateTime? UpdatedAt { get; set; }

        public TaskListDAO TaskList { get; set; } = null!;
        public TaskItemDAO? ParentTask { get; set; }
    }
}
