using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Api.Data;
using PasswordManager.Api.Models;
using PasswordManager.Core.Password;

namespace PasswordManager.Api.Controllers;

[Route("api/passwords")]
[Authorize]
[ApiController]
public class PasswordController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PasswordController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public IActionResult GetPasswordById(int id)
    {
        if (HttpContext.Items["UserId"] is not int userId)
            return Unauthorized();

        var password = _context.Passwords.Find(id);
        if (password == null || password.UserId != userId)
            return NotFound();

        return Ok(password);
    }

    [HttpGet]
    public IActionResult GetPasswords()
    {
        if (HttpContext.Items["UserId"] is not int userId)
            return Unauthorized();

        var passwords = _context.Passwords.Where(p => p.UserId == userId).ToList();
        return Ok(passwords);
    }

    [HttpPost]
    public IActionResult AddPassword([FromBody] CreatePasswordModel createPassword)
    {
        if (HttpContext.Items["UserId"] is not int userId)
            return Unauthorized();

        List<string> validationErrors = PasswordHelper.GetPasswordValidationErrors(createPassword);
        if (validationErrors.Count > 0)
            return BadRequest(validationErrors);

        Password password = new()
        {
            Id = 0,
            Name = createPassword.Name,
            Encrypted = createPassword.Encrypted,
            Category = createPassword.Category,
            UserId = userId,
        };

        _context.Passwords.Add(password);
        _context.SaveChanges();
        return Ok("Password stored successfully");
    }

    [HttpPut("{id}")]
    public IActionResult UpdatePassword(int id, [FromBody] UpdatePasswordModel updatedPassword)
    {
        if (HttpContext.Items["UserId"] is not int userId)
            return Unauthorized();

        var password = _context.Passwords.Find(id);
        if (password == null || password.UserId != userId)
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
        if (HttpContext.Items["UserId"] is not int userId)
            return Unauthorized();

        var password = _context.Passwords.Find(id);
        if (password == null || password.UserId != userId)
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
