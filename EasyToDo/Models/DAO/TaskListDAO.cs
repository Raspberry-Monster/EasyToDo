namespace EasyToDo.Models.DAO
{
    public class TaskListDAO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required Guid OwnerId { get; set; }
        public required string Color { get; set; }
        public DateTime? CreatedAt { get; }
        public DateTime? UpdatedAt { get; set; }

        public UserDAO Owner { get; set; } = null!;
        public List<TaskItemDAO> Items { get; set; } = [];
    }
}
