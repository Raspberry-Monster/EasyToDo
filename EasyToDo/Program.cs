
using EasyToDo.Configurations;
using EasyToDo.Models.DAO;
using EasyToDo.Services;
using EasyToDo.Services.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

namespace EasyToDo
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer 未配置");
            var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience 未配置");
            var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key 未配置");
            var jwtExpiration = builder.Configuration["Jwt:Expiration"] ?? throw new InvalidOperationException("Jwt:Expiration 未配置");
            var servers = builder.Configuration.GetSection("Servers").Get<string[]>() ?? throw new InvalidOperationException("Servers 未配置");
            var expirationInMinutes = int.TryParse(jwtExpiration, out var expiration) ? expiration : throw new InvalidOperationException("Jwt:Expiration 配置无效");
            var keyInBytes = Encoding.UTF8.GetBytes(jwtKey);
            if (keyInBytes.Length < 32)
            {
                throw new InvalidOperationException("JWT 密钥长度不能少于 32 字节");
            }
            builder.Services.AddSingleton(new JWTServiceConfiguration(jwtIssuer, jwtAudience, keyInBytes, expirationInMinutes));
            builder.Services.AddOpenApi();
            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;

                    options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtIssuer,

                        ValidateAudience = true,
                        ValidAudience = jwtAudience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                        RequireSignedTokens = true,
                        ClockSkew = TimeSpan.Zero
                     };
                });
            builder.Services.AddScoped<IPasswordHasher<UserDAO>, PasswordHasher<UserDAO>>();
            builder.Services.AddDbContext<EasyToDoDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
                
            });
            builder.Services.AddScoped<JwtTokenService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<TaskListService>();
            builder.Services.AddScoped<TaskItemService>();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();
            var app = builder.Build();

            app.MapOpenApi()
                .AllowAnonymous();
            app.MapScalarApiReference(options =>
            {
                foreach(var item in servers)
                {
                    options.AddServer(item);
                }
            })
                .AllowAnonymous();

            app.UseExceptionHandler();

            //app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            await using (var scope = app.Services.CreateAsyncScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<EasyToDoDbContext>();
                await dbContext.Database.MigrateAsync();
            }

            await app.RunAsync();
        }
    }
}
