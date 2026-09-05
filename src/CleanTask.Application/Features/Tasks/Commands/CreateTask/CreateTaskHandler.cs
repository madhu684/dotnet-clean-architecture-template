using CleanTask.Application.Common.Exceptions;
using CleanTask.Application.DTOs;
using CleanTask.Domain.Entities;
using CleanTask.Domain.Interfaces;
using MediatR;

namespace CleanTask.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, TaskDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        // Validate creator exists
        var creator = await _unitOfWork.Users.GetByIdAsync(request.CreatedByUserId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.CreatedByUserId);

        // Validate assignee if provided
        User? assignee = null;
        if (request.AssignedToUserId.HasValue)
        {
            assignee = await _unitOfWork.Users.GetByIdAsync(request.AssignedToUserId.Value, cancellationToken)
                ?? throw new NotFoundException(nameof(User), request.AssignedToUserId.Value);
        }

        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            DueDate = request.DueDate,
            AssignedToUserId = request.AssignedToUserId,
            CreatedByUserId = request.CreatedByUserId
        };

        await _unitOfWork.Tasks.AddAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(task, creator, assignee);
    }

    private static TaskDto MapToDto(TaskItem task, User creator, User? assignee) => new(
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
        creator.FullName,
        task.CreatedAt,
        task.UpdatedAt
    );
}
