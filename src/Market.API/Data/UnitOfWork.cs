using Market.API.Data.Interfaces;

namespace Market.API.Data;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public int Commit()
    {
        return context.SaveChanges();
    }

    public void Rollback()
    {
        
    }
}