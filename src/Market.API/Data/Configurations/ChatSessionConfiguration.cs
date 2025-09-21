using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Market.API.Data.Configurations;

public class ChatSessionConfiguration : IEntityTypeConfiguration<ChatSession>
{
    public void Configure(EntityTypeBuilder<ChatSession> builder)
    {
        builder.HasOne(cs => cs.Seller)
            .WithMany(u => u.ChatSellerSessions)
            .HasForeignKey(cs => cs.SellerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(cs => cs.Customer)
            .WithMany(u => u.ChatCustomerSessions)
            .HasForeignKey(cs => cs.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(cs => cs.Product)
            .WithMany(p => p.ChatSessions)
            .HasForeignKey(cs => cs.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}