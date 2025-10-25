using Microsoft.Extensions.Caching.Distributed;

namespace Market.API.Services;

public class OrphanedItemsProcessorService(IRedisKeyService redisKeyService, IUploadFileService uploadFileService, IDistributedCache cache, ILogger<OrphanedItemsProcessorService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("OrphanedItemsProcessorService started at: {time}", DateTimeOffset.Now);
        
        var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await DeleteOrphanedImagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing orphaned items");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
        logger.LogInformation("OrphanedItemsProcessorService stopping at: {time}", DateTimeOffset.Now);
    }
    
    private async Task DeleteOrphanedImagesAsync(CancellationToken cancellationToken = default)
    {
        var keys = await redisKeyService.GetKeysByPrefixAsync($"{Constants.OrphanedImagePrefix}", cancellationToken);
        foreach (var key in keys)
        {
            var imageUrl = await cache.GetStringAsync(key, cancellationToken);
            if (!string.IsNullOrEmpty(imageUrl))
            {
                try
                {
                    await uploadFileService.DeleteFileAsync(imageUrl, "", cancellationToken);
                    logger.LogInformation("Deleted orphaned image {ImageUrl}", imageUrl);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error deleting orphaned image {ImageUrl}", imageUrl);
                }
            }

            await cache.RemoveAsync(key, cancellationToken);
        }
    }
}