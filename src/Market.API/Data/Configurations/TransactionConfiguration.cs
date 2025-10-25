using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Market.API.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasOne(s => s.Product)
            .WithOne(p => p.Transaction)
            .HasForeignKey<Transaction>(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(s => s.Seller)
            .WithMany(u => u.Sales)
            .HasForeignKey(s => s.SellerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(s => s.Buyer)
            .WithMany(u => u.Purchases)
            .HasForeignKey(s => s.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}