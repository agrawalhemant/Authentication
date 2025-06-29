namespace Authentication.Services.Interfaces;

public interface IPhoneService
{
    Task SendVerificationSmsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<(bool status, string message)> VerifyPhoneAsync(Guid userId, string code, CancellationToken cancellationToken = default);

}