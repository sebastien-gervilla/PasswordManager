using Microsoft.AspNetCore.Mvc;
using PasswordManager.Api.Data;
using PasswordManager.Api.Models;

namespace PasswordManager.Api.Controllers;

[Route("api/passwords")]
[ApiController]
public class PasswordController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PasswordController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("{userId}")]
    public IActionResult GetPasswords(int userId)
    {
        var passwords = _context.Passwords.Where(p => p.UserId == userId).ToList();
        return Ok(passwords);
    }

    [HttpPost]
    public IActionResult AddPassword([FromBody] Password password)
    {
        _context.Passwords.Add(password);
        _context.SaveChanges();
        return Ok("Password stored successfully");
    }

    [HttpPut("{id}")]
    public IActionResult UpdatePassword(int id, [FromBody] Password updatedPassword)
    {
        var password = _context.Passwords.Find(id);
        if (password == null)
            return NotFound();

        password.Name = updatedPassword.Name;
        password.Encrypted = updatedPassword.Encrypted;
        password.Category = updatedPassword.Category;
        _context.SaveChanges();

        return Ok("Password updated successfully");
    }

    [HttpDelete("{id}")]
    public IActionResult DeletePassword(int id)
    {
        var password = _context.Passwords.Find(id);
        if (password == null)
            return NotFound();

        _context.Passwords.Remove(password);
        _context.SaveChanges();

        return Ok("Password deleted successfully");
    }

    [HttpGet("search")]
    public IActionResult SearchByCategory([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest("Category name is required.");
        }

        var passwords = _context.Passwords
            .Where(p => p.Category.ToLower().Contains(name.ToLower()))
            .ToList();

        if (!passwords.Any())
        {
            return NotFound("No passwords found for this category.");
        }

        return Ok(passwords);
    }
}
