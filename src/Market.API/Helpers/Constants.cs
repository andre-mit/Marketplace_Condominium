namespace Market.API.Helpers;

public static class Constants
{
    public const string OrphanedImagePrefix = "orphaned-image";

    public const string ProductImagesBucket = "product-images";
    public const string UserAvatarsBucket = "user-avatars";

    private const string OnlineUserCacheKey = "chat:online";
    private const string InChatUserCacheKey = "chat:inchat";
    
    public static string GetOnlineUserCacheKey(Guid userId) => $"{OnlineUserCacheKey}:{userId}";
    public static string GetInChatUserCacheKey(Guid userId, Guid chatSessionId) =>
        $"{InChatUserCacheKey}:{userId}:{chatSessionId}";

    public static readonly Guid AdminRoleId = new("C1C64A20-43D6-440E-9090-1AA2A1CA9A55");
    public static readonly Guid UserRoleId = new("F8A41A51-BFDB-4DCA-ADA9-B025FD2AC2B3");
}