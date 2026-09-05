using CleanTask.Domain.Entities;

namespace CleanTask.Domain.Interfaces;

public interface ITaskRepository : IRepository<TaskItem>
{
    Task<IEnumerable<TaskItem>> GetByAssignedUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskItem>> GetByCreatedUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<TaskItem?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskItem>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
}
