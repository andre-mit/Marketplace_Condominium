using Microsoft.Extensions.Caching.Distributed;

namespace Market.API.Services;

public class OrphanedItemsProcessorService(
    IRedisKeyService redisKeyService,
    IUploadFileService uploadFileService,
    IDistributedCache cache,
    ILogger<OrphanedItemsProcessorService> logger) : BackgroundService
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
        }

        logger.LogInformation("OrphanedItemsProcessorService stopping at: {time}", DateTimeOffset.Now);
    }

    private async Task DeleteOrphanedImagesAsync(CancellationToken cancellationToken = default)
    {
        var keys = await redisKeyService.GetKeysByPrefixAsync($"{Constants.OrphanedImagePrefix}", cancellationToken);

        var keysList = keys.ToList();
        if (keysList.Count == 0)
        {
            logger.LogInformation("No orphaned images found");
            return;
        }

        foreach (var key in keysList)
        {
            var imageUrl = await cache.GetStringAsync(key, cancellationToken);
            if (!string.IsNullOrEmpty(imageUrl))
            {
                try
                {
                    var result = await uploadFileService.DeleteFileAsync(imageUrl, cancellationToken);
                    if (result)
                        logger.LogInformation("Deleted orphaned image {ImageUrl}", imageUrl);
                    else
                        logger.LogWarning("Failed to delete orphaned image {ImageUrl}", imageUrl);
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