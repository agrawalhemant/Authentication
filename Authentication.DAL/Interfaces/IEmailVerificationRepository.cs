using Authentication.DAL.Models;

namespace Authentication.DAL.Interfaces;

public interface IEmailVerificationRepository
{
    Task AddEmailVerificationAsync(EmailVerification emailVerification, CancellationToken cancellationToken = default);
    Task<EmailVerification?> GetEmailVerificationAsync(Guid userId, string code, CancellationToken cancellationToken = default);
    Task UpdateEmailVerificationAsync(EmailVerification emailVerification, CancellationToken cancellationToken = default);
}
