using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Market.API.Data.Configurations;

public class UserConfiguration(IConfiguration configuration) : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasIndex(u => u.CPF)
            .IsUnique();

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.CPF)
            .IsRequired()
            .HasMaxLength(14);

        builder.Property(u => u.Unit)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(u => u.Tower)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<Dictionary<string, object>>(
                "UserRole",
                j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                j =>
                {
                    j.HasKey("UserId", "RoleId");
                    j.ToTable("UserRoles");
                });

        builder.HasMany(u => u.Products)
            .WithOne(p => p.Owner)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new User
            {
                Id = new Guid("A1B2C3D4-E5F6-4789-ABCD-1234567890AB"),
                FirstName = "Admin",
                LastName = "User",
                Email =
                    configuration.GetValue<string>("AdminUser:Email") ?? "admin@admin.com",
                CPF = "000.000.000-00",
                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(configuration.GetValue<string>("AdminUser:Password") ?? "admin123"),
                Birth = new DateOnly(1990, 1, 1),
                Unit = "0",
                Tower = "0"
            });
    }
}