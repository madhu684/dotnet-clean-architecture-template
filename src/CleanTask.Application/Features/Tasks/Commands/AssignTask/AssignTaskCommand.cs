using CleanTask.Application.DTOs;
using MediatR;

namespace CleanTask.Application.Features.Tasks.Commands.AssignTask;

public record AssignTaskCommand(Guid TaskId, Guid? AssignedToUserId) : IRequest<TaskDto>;
