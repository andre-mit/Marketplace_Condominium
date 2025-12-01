using Market.Domain.Enums;

namespace Market.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";

    public required string Cpf { get; set; }

    public required string Email { get; set; }
    public bool IsEmailVerified { get; set; } = false;
    
    public required string Phone { get; set; }
    public bool IsPhoneVerified { get; set; } = false;
    public string? PasswordHash { get; set; }
    
    public string? ResetPasswordCode { get; set; }
    public DateTime? ResetPasswordCodeExpiresAt { get; set; }

    public required DateOnly Birth { get; set; }
    public required string Unit { get; set; }
    public required string Tower { get; set; }

    public UserVerificationStatus VerificationStatus { get; set; } = UserVerificationStatus.Pending;
    
    public byte Rating { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? BannedUntil { get; set; } = null;
    
    public string? AvatarUrl { get; set; } = null;

    public List<Role>? Roles { get; set; }

    public List<Product>? Products { get; set; }
    public List<Transaction>? Sales { get; set; }
    public List<Transaction>? Purchases { get; set; }

    public List<ChatSession>? ChatSellerSessions { get; set; }
    public List<ChatSession>? ChatCustomerSessions { get; set; }

    public List<Rating>? GivenRatings { get; set; }
    public List<Rating>? ReceivedRatings { get; set; }
}