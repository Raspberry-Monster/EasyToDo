using EasyToDo.Models;
using EasyToDo.Models.DTO.Requests;
using EasyToDo.Models.DTO.Responses;
using EasyToDo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace EasyToDo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController(UserService userService) : ControllerBase
    {
        private readonly UserService _userService = userService;

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object>>> RegisterAsync([FromBody] UserRegisterRequest request)
        {
            var result = await _userService.RegisterAsync(request);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<UserLoginResponse>>> LoginAsync([FromBody] UserLoginRequest request)
        {
            var result = await _userService.LoginAsync(request);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("update")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateUserProfileAsync([FromBody] UserProfileUpdateRequest request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userId?.Value))
            {
                return Unauthorized(new { Success = false, Message = "User ID claim is missing." });
            }
            var guidParseStatus = Guid.TryParse(userId.Value, out Guid parsedUserId);
            if (guidParseStatus == false)
            {
                return BadRequest(new { Success = false, Message = "Invalid User ID format." });
            }
            var result = await _userService.UpdateUserProfileAsync(request, parsedUserId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("password")]
        public async Task<ActionResult<ApiResponse<object>>> ChangePasswordAsync([FromBody] UserChangePasswordRequest request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userId?.Value))
            {
                return Unauthorized(new { Success = false, Message = "User ID claim is missing." });
            }
            var guidParseStatus = Guid.TryParse(userId.Value, out Guid parsedUserId);
            if (guidParseStatus == false)
            {
                return BadRequest(new { Success = false, Message = "Invalid User ID format." });
            }
            var result = await _userService.ChangePasswordAsync(request, parsedUserId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
