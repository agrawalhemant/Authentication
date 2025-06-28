namespace Authentication.Contracts.DTOs;

public class UserDto
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }
    public string PreferredLanguage { get; set; } = null!;
}