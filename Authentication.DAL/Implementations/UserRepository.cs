using Authentication.DAL.Interfaces;
using Authentication.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Authentication.DAL.Implementations;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.users.FirstOrDefaultAsync(u => u.email == email, cancellationToken);
    }

    public async Task<bool> UserExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.users.AnyAsync(u => u.email == email, cancellationToken);
    }

    public async Task AddUserAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.users.FindAsync([userId], cancellationToken);
    }

    public async Task UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetUsersCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.users.Where(x => x.isactive).CountAsync(cancellationToken);
    }

    public async Task<List<User>> GetUsersAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.users.Where(x => x.isactive).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    }
}