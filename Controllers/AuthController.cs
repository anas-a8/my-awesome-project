using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // ✅ Log login attempts
            Console.WriteLine($"User login attempt: {request.Username} at {DateTime.UtcNow}");

            if (request.Username == "admin" && request.Password == "admin123")
            {
                var token = GenerateJwtToken(UserRoles.Admin);
                return Ok(new { Token = token });
            }
            else if (request.Username == "user" && request.Password == "user123")
            {
                var token = GenerateJwtToken(UserRoles.User);
                return Ok(new { Token = token });
            }
            return Unauthorized("Invalid username or password");
        }

        private string GenerateJwtToken(UserRoles role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Role, role.ToString()), // ✅ Role claim
                new Claim(JwtRegisteredClaimNames.Aud, _config["Jwt:Audience"]) // ✅ Explicitly setting Audience claim
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"], // ✅ Ensuring Audience matches appsettings.json
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
