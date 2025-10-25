using System.Reflection;
using Market.API.Data.Configurations;
using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Market.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        modelBuilder.ApplyConfiguration(new RatingConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new ChatSessionConfiguration());
        modelBuilder.ApplyConfiguration(new ChatMessageConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Role> Roles { get; set; }
    
    public DbSet<ChatSession> ChatSessions { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
}