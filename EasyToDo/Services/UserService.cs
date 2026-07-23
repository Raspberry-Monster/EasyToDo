using EasyToDo.Models;
using EasyToDo.Models.DAO;
using EasyToDo.Models.DTO.Requests;
using EasyToDo.Models.DTO.Responses;
using EasyToDo.Services.Database;
using EasyToDo.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EasyToDo.Services
{
    public sealed class UserService(EasyToDoDbContext repository, JwtTokenService jwtTokenService, IPasswordHasher<UserDAO> passwordHasher)
    {
        public async Task<ApiResponse<UserLoginResponse>> LoginAsync(UserLoginRequest request)
        {
            var user = await repository.Users.FirstOrDefaultAsync(u => u.UserName == request.Username);
            if (user == null) return ApiResponseFactory.Failure<UserLoginResponse>("Username or Password Incorrect");
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.Password);
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (passwordVerificationResult)
            {
                case PasswordVerificationResult.Failed:
                    return ApiResponseFactory.Failure<UserLoginResponse>("Username or Password Incorrect");
                case PasswordVerificationResult.SuccessRehashNeeded:
                    user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
                    user.UpdatedAt = DateTime.UtcNow;
                    await repository.SaveChangesAsync();
                    break;
            }

            return ApiResponseFactory.Success(ToResponse(user), "Login Successful");
        }

        public async Task<ApiResponse<object>> RegisterAsync(UserRegisterRequest request)
        {
            var userExists = await repository.Users.AnyAsync(u => u.UserName == request.Username);
            if (userExists) return ApiResponseFactory.Failure<object>("User Already Exists");
            var user = new UserDAO
            {
                UserName = request.Username,
                NickName = request.Nickname
            };
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
            repository.Users.Add(user);
            await repository.SaveChangesAsync();
            return ApiResponseFactory.Success<object>(null, "Registration Successful");
        }

        public async Task<ApiResponse<UserProfileResponse>> GetUserProfileAsync(Guid userId)
        {
            var user = await repository.Users.AsNoTracking().SingleOrDefaultAsync(t=>t.Id == userId);
            if (user == null) return ApiResponseFactory.Failure<UserProfileResponse>("User Not Found");
            return ApiResponseFactory.Success(new UserProfileResponse(user.UserName, user.NickName, user.UpdatedAt ?? DateTime.MinValue), "Password Changed Successfully");
        }

        public async Task<ApiResponse<object>> UpdateUserProfileAsync(UserProfileUpdateRequest request, Guid userId)
        {
            var user = await repository.Users.FindAsync(userId);
            if (user == null) return ApiResponseFactory.Failure<object>("User Not Found");
            user.NickName = request.Nickname;
            user.UpdatedAt = DateTime.UtcNow;
            await repository.SaveChangesAsync();
            return ApiResponseFactory.Success<object>(null, "Profile Updated Successfully");
        }

        public async Task<ApiResponse<object>> ChangePasswordAsync(UserChangePasswordRequest request, Guid userId)
        {
            var user = await repository.Users.FindAsync(userId);
            if (user == null) return ApiResponseFactory.Failure<object>("User Not Found");
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.OldPassword);
            if (passwordVerificationResult == PasswordVerificationResult.Failed) return ApiResponseFactory.Failure<object>("Invalid Old Password");
            user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await repository.SaveChangesAsync();
            return ApiResponseFactory.Success<object>(null, "Password Changed Successfully");
        }

        private UserLoginResponse ToResponse(UserDAO user) => jwtTokenService.GenerateResponse(user);
    }
}
