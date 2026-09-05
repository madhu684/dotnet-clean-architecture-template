using CleanTask.Application.DTOs;
using MediatR;

namespace CleanTask.Application.Features.Tasks.Queries.GetAllTasks;

public record GetAllTasksQuery : IRequest<IEnumerable<TaskSummaryDto>>;
