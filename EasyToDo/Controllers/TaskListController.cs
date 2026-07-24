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
        [HttpGet]
        [EndpointDescription("查询全部任务列表")]
        public async Task<ActionResult<ApiResponse<List<TaskListResponse>>>> GetTaskListsAsync()
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskListService.GetTaskListsAsync(userId));
        }

        [HttpGet("{id:guid}")]
        [EndpointDescription("查询任务列表")]
        public async Task<ActionResult<ApiResponse<TaskListDetailResponse>>> GetTaskListAsync(Guid id)
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

        [HttpPost("{id:guid}/update")]
        [EndpointDescription("任务列表更新")]
        public async Task<ActionResult<ApiResponse<List<TaskListResponse>>>> UpdateTaskListAsync(Guid id, [FromBody] TaskListUpdateRequest request)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskListService.UpdateTaskListAsync(id, request, userId));
        }

        [HttpPost("{id:guid}/mark-delete")]
        [EndpointDescription("任务列表标记删除")]
        public async Task<ActionResult<ApiResponse<List<TaskListResponse>>>> MarkDeleteTaskListAsync(Guid id)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskListService.MarkDeleteTaskListAsync(id, userId));
        } 

        [HttpPost("{id:guid}/unmark-delete")]
        [EndpointDescription("任务列表取消标记删除")]
        public async Task<ActionResult<ApiResponse<List<TaskListResponse>>>> UnmarkDeleteTaskListAsync(Guid id)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskListService.UnmarkDeleteTaskListAsync(id, userId));
        }

        [HttpDelete("delete-all")]
        [EndpointDescription("任务列表删除所有已标记的列表")]
        public async Task<ActionResult<ApiResponse<List<TaskListResponse>>>> DeleteAllMarkedTaskListAsync(Guid id)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await taskListService.DeleteAllMarkedTaskListAsync(userId));
        }

        [HttpDelete("{id:guid}")]
        [EndpointDescription("任务列表删除 (需先标记删除)")]
        public async Task<ActionResult<ApiResponse<List<TaskListResponse>>>> DeleteTaskListAsync(Guid id)
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
