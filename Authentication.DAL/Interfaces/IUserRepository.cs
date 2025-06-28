using Authentication.DAL.Models;

namespace Authentication.DAL.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> UserExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddUserAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<int> GetUsersCountAsync(CancellationToken cancellationToken = default);
    Task<List<User>> GetUsersAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}