namespace EasyToDo.Models.DTO.Responses
{
    public record TaskListDetailResponse(string Id, string Name, string Color, List<TaskItemResponse> TaskItems);
}
