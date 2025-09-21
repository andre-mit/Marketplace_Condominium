namespace Domain.Entities;

public class Profile
{
    public Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required DateOnly Birth { get; set; }
    public required string Unit { get; set; }
    public required string Tower { get; set; }
}