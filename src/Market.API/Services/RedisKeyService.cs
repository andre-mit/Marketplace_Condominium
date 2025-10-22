using Market.API.Services.Interfaces;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Market.API.Services;

public class RedisKeyService(IConnectionMultiplexer redis, IOptions<RedisCacheOptions> redisOptions) : IRedisKeyService
{
    private readonly string _instanceName = redisOptions.Value.InstanceName ?? string.Empty;

    public async Task<IEnumerable<string>> GetKeysByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var db = redis.GetDatabase();
        var server = redis.GetServer(redis.GetEndPoints().First());

        var fullPattern = $"{_instanceName}{prefix}*"; 

        var keys = new List<string>();
        await foreach (var key in server.KeysAsync(pattern: fullPattern))
        {
            keys.Add(key.ToString().Replace(_instanceName, ""));
        }
        return keys;
    }
}