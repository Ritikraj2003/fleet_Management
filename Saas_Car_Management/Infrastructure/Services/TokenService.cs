using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Saas_Car_Management.Core.Entities;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateAccessToken(User user, string role, IEnumerable<string> permissions)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, role),
                new Claim("tenant", user.TenantId?.ToString() ?? string.Empty)
            };

            foreach (var perm in permissions)
            {
                claims.Add(new Claim("permissions", perm));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"] ?? "DefaultSuperSecretKeyWithAtLeast32CharactersLong!"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"] ?? "FleetFlowAPI",
                audience: _config["JWT:Audience"] ?? "FleetFlowClient",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["JWT:DurationInMinutes"] ?? "60")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(int userId)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return new RefreshToken
            {
                UserId = userId,
                Token = Convert.ToBase64String(randomNumber),
                Expires = DateTime.UtcNow.AddDays(7)
            };
        }
    }
}
