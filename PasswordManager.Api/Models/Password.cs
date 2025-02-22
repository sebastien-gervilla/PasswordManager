namespace PasswordManager.Api.Models;

public class Password
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Encrypted { get; set; }
    public string Category { get; set; }
    public int UserId { get; set; }
}
