using Microsoft.AspNetCore.Mvc;
using PasswordManager.Api.Data;
using PasswordManager.Api.Models;
using System.Security.Cryptography;
using System.Text;

namespace PasswordManager.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
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

        return Ok("Login successful");
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
