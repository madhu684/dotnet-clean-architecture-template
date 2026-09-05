using CleanTask.Application.Common.Exceptions;
using CleanTask.Application.DTOs;
using CleanTask.Domain.Entities;
using CleanTask.Domain.Interfaces;
using MediatR;

namespace CleanTask.Application.Features.Tasks.Queries.GetTaskById;

public class GetTaskByIdHandler : IRequestHandler<GetTaskByIdQuery, TaskDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTaskByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TaskDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdWithDetailsAsync(request.TaskId, cancellationToken)
            ?? throw new NotFoundException(nameof(TaskItem), request.TaskId);

        if (task.IsDeleted)
            throw new NotFoundException(nameof(TaskItem), request.TaskId);

        return new TaskDto(
            task.Id,
            task.Title,
            task.Description,
            task.Status,
            task.Status.ToString(),
            task.Priority,
            task.Priority.ToString(),
            task.DueDate,
            task.AssignedToUserId,
            task.AssignedTo?.FullName,
            task.CreatedByUserId,
            task.CreatedByUser?.FullName ?? string.Empty,
            task.CreatedAt,
            task.UpdatedAt
        );
    }
}
