using CleanTask.Application.DTOs;
using MediatR;

namespace CleanTask.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password
) : IRequest<UserDto>;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<AuthResponseDto>;
