using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Market.API.Data.Configurations;

public class RatingConfiguration: IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.Property(r => r.Review)
            .IsRequired()
            .HasMaxLength(1000);
        
        builder.Property(r => r.Score)
            .IsRequired();
    }
}