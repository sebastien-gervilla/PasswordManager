namespace PasswordManager.Core.Password;

public class GetPasswordModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Encrypted { get; set; }

    public string Category { get; set; }
}

public class GetPasswordQuery
{
    public string? Name { get; set; }

    public string? Category { get; set; }
}