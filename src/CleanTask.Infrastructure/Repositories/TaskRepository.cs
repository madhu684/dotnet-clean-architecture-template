using CleanTask.Domain.Entities;
using CleanTask.Domain.Interfaces;
using CleanTask.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanTask.Infrastructure.Repositories;

public class TaskRepository : Repository<TaskItem>, ITaskRepository
{
    public TaskRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<TaskItem>> GetByAssignedUserAsync(
        Guid userId, CancellationToken cancellationToken = default)
        => await _context.Tasks
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedByUser)
            .Where(t => t.AssignedToUserId == userId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<TaskItem>> GetByCreatedUserAsync(
        Guid userId, CancellationToken cancellationToken = default)
        => await _context.Tasks
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedByUser)
            .Where(t => t.CreatedByUserId == userId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<TaskItem?> GetByIdWithDetailsAsync(
        Guid id, CancellationToken cancellationToken = default)
        => await _context.Tasks
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedByUser)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<IEnumerable<TaskItem>> GetAllWithDetailsAsync(
        CancellationToken cancellationToken = default)
        => await _context.Tasks
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedByUser)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
}
