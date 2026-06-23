using Li_copy.Models.Users;
using Li_copy.I_InterfaceLayer.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Li_copy.ServiceLayer.Jwt
{
    public class JwtServices : IJwtServices
    {
        private readonly IConfiguration _conn;
        public JwtServices(IConfiguration conn)
        {
            _conn = conn;
        }
        public string GenerateTokenAsync(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("RoleId", user.RoleId.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _conn["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _conn["Jwt:Issuer"],
                audience: _conn["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
        
    }
}
