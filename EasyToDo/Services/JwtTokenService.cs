using EasyToDo.Configurations;
using EasyToDo.Models.DAO;
using EasyToDo.Models.DTO.Responses;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace EasyToDo.Services
{
    public sealed class JwtTokenService
    {
        private readonly JWTServiceConfiguration _configuration;
        private readonly SymmetricSecurityKey _securityKey;
        private readonly SigningCredentials _signingCredential;
        private readonly JsonWebTokenHandler _tokenHandler = new();

        public JwtTokenService(JWTServiceConfiguration configuration)
        {
            _configuration = configuration;
            _securityKey = new(configuration.SecurityKey);
            _signingCredential = new(_securityKey, SecurityAlgorithms.HmacSha256);
        }

        public UserLoginResponse GenerateResponse(UserDAO user)
        {
            var claimsIdentity = new ClaimsIdentity(
         [
             new Claim(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),
            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()),
            new Claim(
                JwtRegisteredClaimNames.Name,
                user.UserName)
         ]);
            var now = DateTime.UtcNow;
            var expiresAt = now.AddMinutes(_configuration.ExpirationInMinutes);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration.Issuer,
                Audience = _configuration.Audience,

                Subject = claimsIdentity,

                IssuedAt = now,
                NotBefore = now,
                Expires = expiresAt,

                SigningCredentials = _signingCredential,

                TokenType = "JWT"
            };
            var accessToken = _tokenHandler.CreateToken(tokenDescriptor);
            return new UserLoginResponse(accessToken, expiresAt);
        }
    }
}
