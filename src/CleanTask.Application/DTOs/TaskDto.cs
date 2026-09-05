using CleanTask.Domain.Enums;

namespace CleanTask.Application.DTOs;

public record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    TaskStatus Status,
    string StatusDisplay,
    TaskPriority Priority,
    string PriorityDisplay,
    DateTime? DueDate,
    Guid? AssignedToUserId,
    string? AssignedToName,
    Guid CreatedByUserId,
    string CreatedByName,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record TaskSummaryDto(
    Guid Id,
    string Title,
    TaskStatus Status,
    string StatusDisplay,
    TaskPriority Priority,
    DateTime? DueDate,
    string? AssignedToName
);
