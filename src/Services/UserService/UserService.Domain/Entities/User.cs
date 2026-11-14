namespace UserService.Domain.Entities;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Role { get; private set; } = "Customer";
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private User() { }

    public static User Create(string fullName, string email, string role = "Customer")
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("Valid email is required.", nameof(email));

        return new User
        {
            FullName = fullName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            Role = string.IsNullOrWhiteSpace(role) ? "Customer" : role.Trim()
        };
    }
}
