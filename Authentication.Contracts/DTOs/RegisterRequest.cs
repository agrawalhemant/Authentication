using System.ComponentModel.DataAnnotations;
using Authentication.Contracts.Enums;

namespace Authentication.Contracts.DTOs;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
    [Required]
    public string FirstName { get; set; } = null!;
    public string? LastName { get; set; }
    
    private string _role = nameof(UserRole.User);
    [EnumDataType(typeof(UserRole))]
    public string Role
    {
        get => _role;
        set => _role = string.IsNullOrWhiteSpace(value) ? nameof(UserRole.User) : value;
    }

    private string _language = nameof(PreferredLanguages.English);
    [EnumDataType(typeof(PreferredLanguages))]
    public string PreferredLanguage
    {
        get => _language;
        set => _language = string.IsNullOrWhiteSpace(value) ? nameof(PreferredLanguages.English) : value;
    }
}