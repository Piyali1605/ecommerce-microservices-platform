namespace UserService.Application.DTOs;

public record UserDto(Guid Id, string FullName, string Email, string Role, DateTime CreatedAt);
public record RegisterUserRequest(string FullName, string Email, string Role = "Customer");
