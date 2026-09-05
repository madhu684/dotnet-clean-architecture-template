using CleanTask.Application.DTOs;
using CleanTask.Domain.Enums;
using MediatR;

namespace CleanTask.Application.Features.Tasks.Commands.UpdateTaskStatus;

public record UpdateTaskStatusCommand(Guid TaskId, TaskStatus NewStatus) : IRequest<TaskDto>;
