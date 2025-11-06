using CleanArch.Domain.Common;

namespace CleanArch.Domain.Entities;

/// <summary>
/// Entidad de usuario para autenticación
/// </summary>
public class User : BaseAuditableEntity
{
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    
    // Relación con roles
    private readonly List<string> _roles = new();
    public IReadOnlyCollection<string> Roles => _roles.AsReadOnly();

    private User() { } // Para EF Core

    private User(string username, string email, string passwordHash, string fullName)
    {
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
        IsActive = true;
    }

    public static User Create(string username, string email, string passwordHash, string fullName)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required", nameof(username));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required", nameof(passwordHash));

        if (username.Length < 3)
            throw new ArgumentException("Username must be at least 3 characters", nameof(username));

        if (!email.Contains("@"))
            throw new ArgumentException("Invalid email format", nameof(email));

        return new User(username, email, passwordHash, fullName);
    }

    public void AddRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role cannot be empty", nameof(role));

        if (!_roles.Contains(role))
            _roles.Add(role);
    }

    public void RemoveRole(string role)
    {
        _roles.Remove(role);
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string fullName, string email)
    {
        if (!string.IsNullOrWhiteSpace(fullName))
            FullName = fullName;

        if (!string.IsNullOrWhiteSpace(email) && email.Contains("@"))
            Email = email;
    }
}
