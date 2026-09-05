using CleanTask.Application.DTOs;
using CleanTask.Domain.Interfaces;
using MediatR;

namespace CleanTask.Application.Features.Users.Queries.GetAllUsers;

public record GetAllUsersQuery : IRequest<IEnumerable<UserDto>>;

public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllUsersHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);

        return users.Select(u => new UserDto(
            u.Id, u.FirstName, u.LastName, u.FullName, u.Email, u.IsActive, u.CreatedAt
        ));
    }
}
