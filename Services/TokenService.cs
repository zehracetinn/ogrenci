using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjeOgrenciYonetim.Web.Models;

namespace ProjeOgrenciYonetim.Web.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateStudentToken(Student s)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, s.Id.ToString()),
                new Claim("role", "student"),
                new Claim("studentId", s.Id.ToString()),
                new Claim("email", s.Email)
            };

            return CreateToken(claims);
        }

        public string CreateAdminToken(AdminUser admin)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, admin.Id.ToString()),
                new Claim("role", "admin"),
                new Claim("userName", admin.UserName)
            };

            return CreateToken(claims);
        }

        private string CreateToken(IEnumerable<Claim> claims)
        {
            var jwt = _config.GetSection("Jwt");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpiresInMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
