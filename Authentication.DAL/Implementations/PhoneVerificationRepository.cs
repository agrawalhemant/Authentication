using Authentication.DAL.Interfaces;
using Authentication.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Authentication.DAL.Implementations;

public class PhoneVerificationRepository : IPhoneVerificationRepository
{
    private readonly AuthDbContext _context;
    public PhoneVerificationRepository(AuthDbContext context)
    {
        _context = context;
    }
    
    public async Task AddPhoneVerificationAsync(PhoneVerification phoneVerification, CancellationToken cancellationToken = default)
    {
        _context.phoneverifications.Add(phoneVerification);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<PhoneVerification?> GetPhoneVerificationAsync(Guid userId, string code, CancellationToken cancellationToken = default)
    {
        return await _context.phoneverifications.FirstOrDefaultAsync(x =>
            x.userid == userId && x.otp == code, cancellationToken: cancellationToken);
    }

    public async Task UpdatePhoneVerificationAsync(PhoneVerification phoneVerification, CancellationToken cancellationToken = default)
    {
        _context.phoneverifications.Update(phoneVerification);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ExpireAllCodesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var verifications = await _context.phoneverifications
            .Where(x => x.userid == userId && !x.isused)
            .ToListAsync(cancellationToken);

        verifications.ForEach(verification => verification.expiresat = DateTime.UtcNow.AddDays(-1));
        await _context.SaveChangesAsync(cancellationToken);
    }
}