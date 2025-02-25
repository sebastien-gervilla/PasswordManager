using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PasswordManager.Api.Data;
using PasswordManager.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PasswordManager.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] User user)
    {
        if (_context.Users.Any(u => u.Email == user.Email))
            return BadRequest("Email already registered");

        user.Password = HashPassword(user.Password);
        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok("User registered successfully");
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] User user)
    {
        var dbUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
        if (dbUser == null || dbUser.Password != HashPassword(user.Password))
            return Unauthorized("Invalid credentials");

        var token = GenerateJwtToken(dbUser.Id, dbUser.Email);
        return Ok(new { Token = token });
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }

    private string GenerateJwtToken(int id, string email)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.NameId, id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
