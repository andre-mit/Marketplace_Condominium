namespace Market.API.Data.Interfaces;

public interface IUnitOfWork
{
    Task<int> CommitAsync();
    int Commit();
    void Rollback();
}