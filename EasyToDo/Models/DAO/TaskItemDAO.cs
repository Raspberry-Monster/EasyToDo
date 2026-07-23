using EasyToDo.Configurations;
using EasyToDo.Models.Enums;
using Microsoft.EntityFrameworkCore;
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength
// ReSharper disable InconsistentNaming
// ReSharper disable EntityFramework.ModelValidation.CircularDependency

namespace EasyToDo.Models.DAO
{
    [EntityTypeConfiguration(typeof(TaskItemDAOConfiguration))]
    public class TaskItemDAO
    {
        public Guid Id { get; set; }
        public required Guid ListId { get; set; }
        public required Guid OwnerId { get; set; }
        public Guid? ParentTaskId { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public TaskItemStatus Status { get; set; } = TaskItemStatus.Stopped;
        public TaskItemPriority Priority { get; set; } = TaskItemPriority.Normal;
        public int Progress { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? NotifyAt { get; set; }
        public DateTime? DueAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public UserDAO Owner { get; set; } = null!;
        public TaskListDAO TaskList { get; set; } = null!;
        public TaskItemDAO? ParentTask { get; set; }
        public List<TaskItemDAO> SubTasks { get; set; } = [];
    }
}
