using EasyToDo.Models;
using EasyToDo.Models.DAO;
using EasyToDo.Models.DTO.Requests;
using EasyToDo.Models.DTO.Responses;
using EasyToDo.Services.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EasyToDo.Services
{
    public sealed class UserService(EasyToDoDbContext repository, JwtTokenService jwtTokenService, IPasswordHasher<UserDAO> passwordHasher)
    {
        public async Task<ApiResponse<UserLoginResponse>> LoginAsync(UserLoginRequest request)
        {
            var user = await repository.Users.FirstOrDefaultAsync(u => u.UserName == request.Username);
            if (user == null) return new ApiResponse<UserLoginResponse> { Success = false, Message = "User Not Found" };
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.Password);
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (passwordVerificationResult)
            {
                case PasswordVerificationResult.Failed:
                    return new ApiResponse<UserLoginResponse> { Success = false, Message = "Invalid Password" };
                case PasswordVerificationResult.SuccessRehashNeeded:
                    user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
                    repository.Users.Update(user);
                    await repository.SaveChangesAsync();
                    break;
            }

            var loginResponse = jwtTokenService.GenerateResponse(user);
            return new ApiResponse<UserLoginResponse> { Success = true, Message = "Login Successful", Data = loginResponse };
        }

        public async Task<ApiResponse<object>> RegisterAsync(UserRegisterRequest request)
        {
            var userExists = await repository.Users.AnyAsync(u => u.UserName == request.Username);
            if (userExists) return new ApiResponse<object> { Success = false, Message = "User Already Exists" };
            var user = new UserDAO
            {
                UserName = request.Username,
                NickName = request.Nickname
            };
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
            repository.Users.Add(user);
            await repository.SaveChangesAsync();
            return new ApiResponse<object> { Success = true, Message = "Registration Successful" };
        }

        public async Task<ApiResponse<object>> UpdateUserProfileAsync(UserProfileUpdateRequest request, Guid userId)
        {
            var user = await repository.Users.FindAsync(userId);
            if (user == null) return new ApiResponse<object> { Success = false, Message = "User Not Found" };
            user.NickName = request.Nickname;
            user.UpdatedAt = DateTime.UtcNow;
            await repository.SaveChangesAsync();
            return new ApiResponse<object> { Success = true, Message = "Profile Updated Successfully" };
        }

        public async Task<ApiResponse<object>> ChangePasswordAsync(UserChangePasswordRequest request, Guid userId)
        {
            var user = await repository.Users.FindAsync(userId);
            if (user == null) return new ApiResponse<object> { Success = false, Message = "User Not Found" };
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.OldPassword);
            if (passwordVerificationResult == PasswordVerificationResult.Failed) return new ApiResponse<object> { Success = false, Message = "Invalid Old Password" };
            user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await repository.SaveChangesAsync();
            return new ApiResponse<object> { Success = true, Message = "Password Changed Successfully" };
        }
    }
}
