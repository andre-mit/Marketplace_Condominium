namespace Market.Domain.Enums;

public enum UserVerificationStatus : byte
{
    Pending,
    PendingManualReview,
    Verified,
    Rejected,
    Banned
}