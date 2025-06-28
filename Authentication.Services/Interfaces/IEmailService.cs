namespace Authentication.Services.Interfaces;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string toEmail, string firstName, CancellationToken cancellationToken = default);
    Task SendVerificationEmailAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<(bool status, string message)> VerifyEmailAsync(Guid userId, string code, CancellationToken cancellationToken = default);
}