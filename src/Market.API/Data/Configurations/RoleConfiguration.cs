using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Market.API.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(r => r.Name)
            .IsUnique();
        
        builder.HasData(
            new Role { Id = new Guid("C1C64A20-43D6-440E-9090-1AA2A1CA9A55"), Name = "Admin" },
            new Role { Id = new Guid("F8A41A51-BFDB-4DCA-ADA9-B025FD2AC2B3"), Name = "User" }
        );
    }
}