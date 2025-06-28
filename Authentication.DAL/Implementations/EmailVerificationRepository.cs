using Authentication.DAL.Interfaces;
using Authentication.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Authentication.DAL.Implementations;

public class EmailVerificationRepository : IEmailVerificationRepository
{
    private readonly AuthDbContext _context;
    public EmailVerificationRepository(AuthDbContext context)
    {
        _context = context;
    }
    
    public async Task AddEmailVerificationAsync(EmailVerification emailVerification, CancellationToken cancellationToken = default)
    {
        _context.emailverifications.Add(emailVerification);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<EmailVerification?> GetEmailVerificationAsync(Guid userId, string code, CancellationToken cancellationToken = default)
    {
        return await _context.emailverifications.FirstOrDefaultAsync(x =>
            x.userid == userId && x.verificationtoken == code, cancellationToken: cancellationToken);
    }

    public async Task UpdateEmailVerificationAsync(EmailVerification emailVerification, CancellationToken cancellationToken = default)
    {
        _context.emailverifications.Update(emailVerification);
        await _context.SaveChangesAsync(cancellationToken);
    }
}