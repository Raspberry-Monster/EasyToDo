namespace EasyToDo.Models.DAO
{
    public class UserDAO
    {
        public Guid Id { get; set; }
        public required string UserName { get; set; }
        public required string NickName { get; set; }
        public required string PasswordHash { get; set; }
        public DateTime? CreatedAt { get; }
        public DateTime? UpdatedAt { get; set; }

        public List<TaskListDAO> TaskLists { get; set; } = [];
    }
}
