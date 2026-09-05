using CleanTask.Application.DTOs;
using MediatR;

namespace CleanTask.Application.Features.Tasks.Queries.GetTaskById;

public record GetTaskByIdQuery(Guid TaskId) : IRequest<TaskDto>;
