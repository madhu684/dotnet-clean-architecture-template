using CleanTask.Application.Common.Exceptions;
using CleanTask.Application.DTOs;
using CleanTask.Domain.Entities;
using CleanTask.Domain.Interfaces;
using MediatR;

namespace CleanTask.Application.Features.Tasks.Commands.AssignTask;

public class AssignTaskHandler : IRequestHandler<AssignTaskCommand, TaskDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public AssignTaskHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TaskDto> Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdWithDetailsAsync(request.TaskId, cancellationToken)
            ?? throw new NotFoundException(nameof(TaskItem), request.TaskId);

        User? assignee = null;
        if (request.AssignedToUserId.HasValue)
        {
            assignee = await _unitOfWork.Users.GetByIdAsync(request.AssignedToUserId.Value, cancellationToken)
                ?? throw new NotFoundException(nameof(User), request.AssignedToUserId.Value);
        }

        task.AssignedToUserId = request.AssignedToUserId;
        task.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Tasks.UpdateAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
            assignee?.FullName,
            task.CreatedByUserId,
            task.CreatedByUser?.FullName ?? string.Empty,
            task.CreatedAt,
            task.UpdatedAt
        );
    }
}
