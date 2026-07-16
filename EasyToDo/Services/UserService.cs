using EasyToDo.Models;
using EasyToDo.Models.DAO;
using EasyToDo.Models.DTO.Requests;
using EasyToDo.Models.DTO.Responses;
using EasyToDo.Services.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyToDo.Services
{
    public sealed class UserService(EasyToDoDbContext repository, JwtTokenService jwtTokenService, IPasswordHasher<UserDAO> passwordHasher)
    {
        private readonly EasyToDoDbContext _repository = repository;
        private readonly JwtTokenService _jwtTokenService = jwtTokenService;
        private readonly IPasswordHasher<UserDAO> _passwordHasher = passwordHasher;

        public async Task<ApiResponse<UserLoginResponse>> LoginAsync(UserLoginRequest request)
        {
            var user = await _repository.Users.FirstOrDefaultAsync(u => u.UserName == request.Username);
            if (user == null) return new ApiResponse<UserLoginResponse> { Success = false, Message = "User Not Found" };
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.Password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed) return new ApiResponse<UserLoginResponse> { Success = false, Message = "Invalid Password" };
            if (passwordVerificationResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
                _repository.Users.Update(user);
                await _repository.SaveChangesAsync();
            }
            var loginResponse = _jwtTokenService.GenerateResponse(user);
            return new ApiResponse<UserLoginResponse> { Success = true, Message = "Login Successful", Data = loginResponse };
        }

        public async Task<ApiResponse<object>> RegisterAsync(UserRegisterRequest request)
        {
            var userExists = await _repository.Users.AnyAsync(u => u.UserName == request.Username);
            if (userExists) return new ApiResponse<object> { Success = false, Message = "User Already Exists" };
            var user = new UserDAO
            {
                UserName = request.Username,
                NickName = request.Nickname
            };
            _passwordHasher.HashPassword(user, request.Password);
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            _repository.Users.Add(user);
            await _repository.SaveChangesAsync();
            var loginResponse = _jwtTokenService.GenerateResponse(user);
            return new ApiResponse<object> { Success = true, Message = "Registration Successful" };
        }

        public async Task<ApiResponse<object>> UpdateUserProfileAsync(UserProfileUpdateRequest request, Guid userId)
        {
            var user = await _repository.Users.FindAsync(userId);
            if (user == null) return new ApiResponse<object> { Success = false, Message = "User Not Found" };
            user.NickName = request.Nickname;
            user.UpdatedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();
            return new ApiResponse<object> { Success = true, Message = "Profile Updated Successfully" };
        }

        public async Task<ApiResponse<object>> ChangePasswordAsync(UserChangePasswordRequest request, Guid userId)
        {
            var user = await _repository.Users.FindAsync(userId);
            if (user == null) return new ApiResponse<object> { Success = false, Message = "User Not Found" };
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.OldPassword);
            if (passwordVerificationResult == PasswordVerificationResult.Failed) return new ApiResponse<object> { Success = false, Message = "Invalid Old Password" };
            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();
            return new ApiResponse<object> { Success = true, Message = "Password Changed Successfully" };
        }
    }
}
