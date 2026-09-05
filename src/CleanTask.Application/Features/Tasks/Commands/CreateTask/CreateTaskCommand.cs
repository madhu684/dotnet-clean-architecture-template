using CleanTask.Application.DTOs;
using CleanTask.Domain.Enums;
using MediatR;

namespace CleanTask.Application.Features.Tasks.Commands.CreateTask;

public record CreateTaskCommand(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate,
    Guid? AssignedToUserId,
    Guid CreatedByUserId
) : IRequest<TaskDto>;
