using EasyToDo.Models;
using EasyToDo.Models.DTO.Requests;
using EasyToDo.Models.DTO.Responses;
using EasyToDo.Services;
using EasyToDo.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyToDo.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UserController(UserService userService) : ControllerBase
    {
        [EndpointDescription("用户注册")]
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object>>> RegisterAsync([FromBody] UserRegisterRequest request) => this.ToActionResult(await userService.RegisterAsync(request));

        [EndpointDescription("用户登录")]
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<UserLoginResponse>>> LoginAsync([FromBody] UserLoginRequest request) => this.ToActionResult(await userService.LoginAsync(request));

        [EndpointDescription("用户信息更新")]
        [HttpPost("update")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateUserProfileAsync([FromBody] UserProfileUpdateRequest request)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await userService.UpdateUserProfileAsync(request, userId));
        }

        [EndpointDescription("密码修改")]
        [HttpPost("password")]
        public async Task<ActionResult<ApiResponse<object>>> ChangePasswordAsync([FromBody] UserChangePasswordRequest request)
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await userService.ChangePasswordAsync(request, userId));
        }

        [EndpointDescription("用户信息获取")]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<UserProfileResponse>>> GetUserInfoAsync()
        {
            if (this.GetUserId(out var userId) is { } error) return error;
            return this.ToActionResult(await userService.GetUserProfileAsync(userId));
        }
    }
}
