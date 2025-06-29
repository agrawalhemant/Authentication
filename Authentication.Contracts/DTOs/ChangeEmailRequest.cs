namespace Authentication.Contracts.DTOs;

public class ChangeEmailRequest
{
    public string NewEmail { get; set; } = null!;
    public string ConfirmEmail { get; set; } = null!;
}