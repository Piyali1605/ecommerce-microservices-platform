using UserService.Application.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.Services;

public interface IUserManagementService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken ct = default);
    Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken ct = default);
    Task<UserDto> RegisterUserAsync(RegisterUserRequest request, CancellationToken ct = default);
}

public class UserManagementService : IUserManagementService
{
    private readonly IUserRepository _repository;

    public UserManagementService(IUserRepository repository) => _repository = repository;

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken ct = default) =>
        (await _repository.GetAllAsync(ct)).Select(u => new UserDto(u.Id, u.FullName, u.Email, u.Role, u.CreatedAt));

    public async Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken ct = default)
    {
        var u = await _repository.GetByIdAsync(id, ct);
        return u is null ? null : new UserDto(u.Id, u.FullName, u.Email, u.Role, u.CreatedAt);
    }

    public async Task<UserDto> RegisterUserAsync(RegisterUserRequest request, CancellationToken ct = default)
    {
        if (await _repository.GetByEmailAsync(request.Email, ct) is not null)
            throw new InvalidOperationException($"Email '{request.Email}' is already registered.");

        var saved = await _repository.AddAsync(User.Create(request.FullName, request.Email, request.Role), ct);
        return new UserDto(saved.Id, saved.FullName, saved.Email, saved.Role, saved.CreatedAt);
    }
}
