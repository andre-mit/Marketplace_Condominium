namespace Market.API.Data.Interfaces;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    int Commit();
    void Rollback();
}