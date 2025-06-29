using Authentication.DAL.Models;

namespace Authentication.DAL.Interfaces;

public interface IPhoneVerificationRepository
{
    Task AddPhoneVerificationAsync(PhoneVerification phoneVerification, CancellationToken cancellationToken = default);
    Task<PhoneVerification?> GetPhoneVerificationAsync(Guid userId, string code, CancellationToken cancellationToken = default);
    Task UpdatePhoneVerificationAsync(PhoneVerification phoneVerification, CancellationToken cancellationToken = default);
    Task ExpireAllCodesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}