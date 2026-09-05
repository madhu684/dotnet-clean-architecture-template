using CleanTask.Application.Common.Exceptions;
using CleanTask.Application.DTOs;
using CleanTask.Domain.Entities;
using CleanTask.Domain.Interfaces;
using MediatR;

namespace CleanTask.Application.Features.Tasks.Commands.UpdateTaskStatus;

public class UpdateTaskStatusHandler : IRequestHandler<UpdateTaskStatusCommand, TaskDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTaskStatusHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TaskDto> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdWithDetailsAsync(request.TaskId, cancellationToken)
            ?? throw new NotFoundException(nameof(TaskItem), request.TaskId);

        task.Status = request.NewStatus;
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
            task.AssignedTo?.FullName,
            task.CreatedByUserId,
            task.CreatedByUser?.FullName ?? string.Empty,
            task.CreatedAt,
            task.UpdatedAt
        );
    }
}
