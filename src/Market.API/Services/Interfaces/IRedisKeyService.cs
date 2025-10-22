namespace Market.API.Services.Interfaces;

public interface IRedisKeyService
{
    Task<IEnumerable<string>> GetKeysByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
}