using Authentication.Contracts.Config;
using Authentication.DAL.Interfaces;
using Authentication.DAL.Models;
using Authentication.Services.Interfaces;
using Authentication.Utility;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Authentication.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly SendGridSettings _settings;
    private readonly IUserRepository _userRepository;
    private readonly IEmailVerificationRepository _emailVerificationRepository;

    public EmailService(IUserRepository userRepository, IEmailVerificationRepository emailVerificationRepository, IOptions<SendGridSettings> settings)
    {
        _userRepository = userRepository;
        _emailVerificationRepository = emailVerificationRepository;
        _settings = settings.Value;
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string firstName, CancellationToken cancellationToken = default)
    {
        const string subject = "Welcome to Authentication App";
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "WelcomeEmail.html");
        var htmlTemplate = await File.ReadAllTextAsync(path, cancellationToken);
        
        var emailHtml = htmlTemplate
            .Replace("{{FirstName}}", firstName)
            .Replace("{{AppName}}", "Authentication App")
            .Replace("{{CurrentYear}}", DateTime.UtcNow.Year.ToString());
        await SendEmailAsync(toEmail, subject, emailHtml, cancellationToken);
    }

    public async Task SendVerificationEmailAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
        if(user is null)
            throw new Exception("User not found.");
        
        var code = Verification.GenerateVerificationCode();
        var expiresAt = DateTime.UtcNow.AddMinutes(10);
        var emailVerification = new EmailVerification
        {
            id = Guid.NewGuid(),
            userid = userId,
            verificationtoken = code,
            expiresat = expiresAt,
            isused = false,
            createdat = DateTime.UtcNow
        };
        await _emailVerificationRepository.AddEmailVerificationAsync(emailVerification, cancellationToken);
        
        const string subject = "Verify your email address";
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "VerificationCodeEmail.html");
        var htmlTemplate = await File.ReadAllTextAsync(path, cancellationToken);
        var emailHtml = htmlTemplate
            .Replace("{{UserName}}", user.firstname + " " + user.lastname)
            .Replace("{{VerificationCode}}", code)
            .Replace("{{AppName}}", "Authentication App")
            .Replace("{{CurrentYear}}", DateTime.UtcNow.Year.ToString());
        await SendEmailAsync(user.email, subject, emailHtml, cancellationToken );
    }
    
    public async Task<(bool status, string message)> VerifyEmailAsync(Guid userId, string code, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
        if(user is null)
            return (false, "User not found.");
        var emailVerification = await _emailVerificationRepository.GetEmailVerificationAsync(userId, code, cancellationToken);
        if(emailVerification is null)
            return (false, "Verification code is invalid.");
        if(emailVerification.isused)
            return (false, "Verification code is already used.");
        if(emailVerification.expiresat < DateTime.UtcNow)
            return (false, "Verification code has expired.");
        
        emailVerification.isused = true;
        emailVerification.expiresat = DateTime.UtcNow.AddMinutes(-1);
        await _emailVerificationRepository.UpdateEmailVerificationAsync(emailVerification, cancellationToken);
        
        user.isemailverified = true;
        await _userRepository.UpdateUserAsync(user, cancellationToken);
        
        const string subject = "Email verification Successful";
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "SuccessfulVerificationEmail.html");
        var htmlTemplate = await File.ReadAllTextAsync(path, cancellationToken);
        var emailHtml = htmlTemplate
            .Replace("{{UserName}}", user.firstname + " " + user.lastname)
            .Replace("{{AppName}}", "Authentication App")
            .Replace("{{CurrentYear}}", DateTime.UtcNow.Year.ToString());
        await SendEmailAsync(user.email, subject, emailHtml, cancellationToken );
        
        return (true, "Email verified successfully.");
    }
    
    #region Private Methods
    private async Task SendEmailAsync(string toEmail, string subject, string emailHtml, CancellationToken cancellationToken = default)
    {
        var client = new SendGridClient(_settings.ApiKey);

        var from = new EmailAddress(_settings.FromEmail, _settings.FromName);
        var to = new EmailAddress(toEmail);
        
        var msg = MailHelper.CreateSingleEmail(from, to, subject, "", emailHtml);
        var response = await client.SendEmailAsync(msg, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to send email. Status: {response.StatusCode}");
        }
    }
    #endregion
}