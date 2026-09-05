namespace CleanTask.Application.DTOs;

public record UserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    bool IsActive,
    DateTime CreatedAt
);

public record AuthResponseDto(
    string Token,
    string Email,
    string FullName,
    Guid UserId,
    DateTime ExpiresAt
);
