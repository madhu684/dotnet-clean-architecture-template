using CleanTask.Domain.Entities;
using CleanTask.Domain.Interfaces;
using CleanTask.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanTask.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => await _context.Users
            .AnyAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
}
