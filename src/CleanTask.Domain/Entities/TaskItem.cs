using CleanTask.Domain.Common;
using CleanTask.Domain.Enums;
using TaskStatus = CleanTask.Domain.Enums.TaskStatus;

namespace CleanTask.Domain.Entities;

public class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; set; }
    public bool IsDeleted { get; set; } = false;

    // Relationships
    public Guid? AssignedToUserId { get; set; }
    public User? AssignedTo { get; set; }

    public Guid CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }
}
