using CleanTask.Application.DTOs;
using CleanTask.Domain.Interfaces;
using MediatR;

namespace CleanTask.Application.Features.Tasks.Queries.GetAllTasks;

public class GetAllTasksHandler : IRequestHandler<GetAllTasksQuery, IEnumerable<TaskSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllTasksHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TaskSummaryDto>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
    {
        var tasks = await _unitOfWork.Tasks.GetAllWithDetailsAsync(cancellationToken);

        return tasks
            .Where(t => !t.IsDeleted)
            .Select(t => new TaskSummaryDto(
                t.Id,
                t.Title,
                t.Status,
                t.Status.ToString(),
                t.Priority,
                t.DueDate,
                t.AssignedTo?.FullName
            ));
    }
}
