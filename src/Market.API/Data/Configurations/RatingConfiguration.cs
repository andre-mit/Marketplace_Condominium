using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Market.API.Data.Configurations;

public class RatingConfiguration: IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.HasKey(r => new { r.TransactionId, r.RaterId, r.RatedId });
        
        builder.HasOne(r => r.Transaction)
            .WithMany(t => t.Ratings)
            .HasForeignKey(r => r.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(r => r.Rater)
            .WithMany(u => u.GivenRatings)
            .HasForeignKey(r => r.RaterId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(r => r.Rated)
            .WithMany(u => u.ReceivedRatings)
            .HasForeignKey(r => r.RatedId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(r => r.Review)
            .IsRequired()
            .HasMaxLength(1000);
        
        builder.Property(r => r.Score)
            .IsRequired();
    }
}