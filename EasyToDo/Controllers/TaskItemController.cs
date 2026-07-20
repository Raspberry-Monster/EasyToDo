using EasyToDo.Models;
using EasyToDo.Models.DTO.Requests;
using EasyToDo.Models.DTO.Responses;
using EasyToDo.Services;
using EasyToDo.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyToDo.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    [Authorize]
    public class TaskItemController(TaskItemService taskItemService) : ControllerBase
    {
        [HttpGet]
        [EndpointDescription("查询任务")]
        public async Task<ActionResult<ApiResponse<List<TaskItemResponse>>>> GetTasksAsync()
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskItemService.GetTaskItemsAsync(userId));
        }

        [HttpPost]
        [EndpointDescription("创建任务")]
        public async Task<ActionResult<ApiResponse<List<TaskItemResponse>>>> CreateTaskAsync([FromBody] TaskItemCreateRequest request)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskItemService.CreateTaskAsync(request, userId));
        }

        [HttpGet("{id}")]
        [EndpointDescription("获取任务详情")]
        public async Task<ActionResult<ApiResponse<TaskItemDetailResponse>>> GetTaskAsync(string id)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskItemService.GetTaskAsync(id, userId));
        }

        [HttpPost("{id}/update")]
        [EndpointDescription("修改任务完整信息")]
        public async Task<ActionResult<ApiResponse<TaskItemDetailResponse>>> UpdateTaskAsync(string id, [FromBody] TaskItemUpdateRequest request)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskItemService.UpdateTaskAsync(id, request, userId));
        }

        [HttpPost("{id}/status")]
        [EndpointDescription("修改任务状态")]
        public async Task<ActionResult<ApiResponse<TaskItemDetailResponse>>> UpdateTaskStatusAsync(string id, [FromBody] TaskItemStatusUpdateRequest request)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskItemService.UpdateTaskStatusAsync(id, request, userId, false));
        }

        [HttpPost("{id}/progress")]
        [EndpointDescription("修改任务进度")]
        public async Task<ActionResult<ApiResponse<TaskItemDetailResponse>>> UpdateTaskProgressAsync(string id, [FromBody] TaskItemProgressUpdateRequest request)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskItemService.UpdateTaskProgressAsync(id, request, userId));
        }

        [HttpPost("{id}/unmark-delete")]
        [EndpointDescription("从回收站恢复任务")]
        public async Task<ActionResult<ApiResponse<List<TaskItemResponse>>>> UnmarkDeleteTaskAsync(string id)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskItemService.UnmarkDeleteTaskAsync(id, userId));
        }

        [HttpPost("{id}/mark-delete")]
        [EndpointDescription("将任务移入回收站")]
        public async Task<ActionResult<ApiResponse<List<TaskItemResponse>>>> MarkDeleteTaskAsync(string id)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskItemService.MarkDeleteTaskAsync(id, userId));
        }

        [HttpDelete("{id}")]
        [EndpointDescription("永久删除任务")]
        public async Task<ActionResult<ApiResponse<List<TaskItemResponse>>>> DeleteTaskAsync(string id)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskItemService.DeleteTaskAsync(id, userId));
        }

        [HttpGet("completed")]
        [EndpointDescription("获取已完成任务")]
        public async Task<ActionResult<ApiResponse<List<TaskItemResponse>>>> GetCompletedTasksAsync()
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskItemService.GetCompletedTasksAsync(userId));
        }

        [HttpGet("deleted")]
        [EndpointDescription("获取回收站任务")]
        public async Task<ActionResult<ApiResponse<List<TaskItemResponse>>>> GetDeletedTasksAsync()
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskItemService.GetDeletedTasksAsync(userId));
        }

        [HttpGet("calendar")]
        [EndpointDescription("获取指定日期范围内的任务")]
        public async Task<ActionResult<ApiResponse<List<TaskItemResponse>>>> GetCalendarTasksAsync([FromQuery] DateTime startAt, [FromQuery] DateTime endAt)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskItemService.GetCalendarTasksAsync(startAt, endAt, userId));
        }

        [HttpGet("{taskId}/subtasks")]
        [EndpointDescription("获取指定任务的全部子任务")]
        public async Task<ActionResult<ApiResponse<List<TaskItemResponse>>>> GetSubTasksAsync(string taskId)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskItemService.GetSubTasksAsync(taskId, userId));
        }

        [HttpPost("{taskId}/subtasks")]
        [EndpointDescription("创建子任务")]
        public async Task<ActionResult<ApiResponse<List<TaskItemResponse>>>> CreateSubTaskAsync(string taskId, [FromBody] SubTaskCreateRequest request)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskItemService.CreateSubTaskAsync(taskId, request, userId));
        }

        [HttpGet("/api/subtasks/{subtaskId}")]
        [EndpointDescription("获取子任务详情")]
        public async Task<ActionResult<ApiResponse<TaskItemDetailResponse>>> GetSubTaskAsync(string subtaskId)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskItemService.GetSubTaskAsync(subtaskId, userId));
        }

        [HttpPost("/api/subtasks/{subtaskId}/update")]
        [EndpointDescription("修改子任务")]
        public async Task<ActionResult<ApiResponse<TaskItemDetailResponse>>> UpdateSubTaskAsync(string subtaskId, [FromBody] TaskItemUpdateRequest request)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskItemService.UpdateSubTaskAsync(subtaskId, request, userId));
        }

        [HttpPost("/api/subtasks/{subtaskId}/status")]
        [EndpointDescription("修改子任务状态")]
        public async Task<ActionResult<ApiResponse<TaskItemDetailResponse>>> UpdateSubTaskStatusAsync(string subtaskId, [FromBody] TaskItemStatusUpdateRequest request)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskItemService.UpdateTaskStatusAsync(subtaskId, request, userId, true));
        }

        [HttpDelete("/api/subtasks/{subtaskId}")]
        [EndpointDescription("删除子任务")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteSubTaskAsync(string subtaskId)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskItemService.DeleteSubTaskAsync(subtaskId, userId));
        }
    }
}
