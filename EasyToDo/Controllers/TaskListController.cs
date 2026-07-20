using EasyToDo.Models;
using EasyToDo.Models.DTO.Requests;
using EasyToDo.Models.DTO.Responses;
using EasyToDo.Services;
using EasyToDo.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyToDo.Controllers
{
    [Route("api/tasklist")]
    [ApiController]
    [Authorize]
    public class TaskListController(TaskListService taskListService) : ControllerBase
    {
        [HttpGet("{id}")]
        [EndpointDescription("查询任务列表")]
        public async Task<ActionResult<ApiResponse<TaskListDetailResponse>>> GetTaskListAsync(string id)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskListService.GetTaskListAsync(id, userId));
        }

        [HttpPost("create")]
        [EndpointDescription("任务列表创建")]
        public async Task<ActionResult<ApiResponse<List<TaskListResponse>>>> CreateTaskListAsync([FromBody] TaskListCreateRequest request)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskListService.CreateTaskListAsync(request, userId));
        }

        [HttpPost("{id}/update")]
        [EndpointDescription("任务列表更新")]
        public async Task<ActionResult<ApiResponse<List<TaskListResponse>>>> UpdateTaskListAsync(string id, [FromBody] TaskListUpdateRequest request)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskListService.UpdateTaskListAsync(id, request, userId));
        }

        [HttpPost("{id}/mark-delete")]
        [EndpointDescription("任务列表标记删除")]
        public async Task<ActionResult<ApiResponse<List<TaskListResponse>>>> MarkDeleteTaskListAsync(string id)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskListService.MarkDeleteTaskListAsync(id, userId));
        }

        [HttpPost("{id}/unmark-delete")]
        [EndpointDescription("任务列表取消标记删除")]
        public async Task<ActionResult<ApiResponse<List<TaskListResponse>>>> UnmarkDeleteTaskListAsync(string id)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskListService.UnmarkDeleteTaskListAsync(id, userId));
        }

        [HttpDelete("{id}")]
        [EndpointDescription("任务列表删除 (需先标记删除)")]
        public async Task<ActionResult<ApiResponse<List<TaskListResponse>>>> DeleteTaskListAsync(string id)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskListService.DeleteTaskListAsync(id, userId));
        }

        [HttpGet("deleted")]
        [EndpointDescription("已删除任务列表获取")]
        public async Task<ActionResult<ApiResponse<List<TaskListResponse>>>> GetDeletedTaskAsync()
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskListService.GetDeletedTaskListAsync(userId));
        }
    }
}
