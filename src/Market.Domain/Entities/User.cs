namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    
    public required string Email { get; set; }
    public string PasswordHash { get; set; }

    public required DateOnly Birth { get; set; }
    public required string Unit { get; set; }
    public required string Tower { get; set; }

    public List<Role> Roles { get; set; }
}