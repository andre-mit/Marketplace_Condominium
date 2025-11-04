using Market.Domain.Enums;

namespace Market.SharedApplication.ViewModels.TransactionVIewModels;

public class RateTransactionViewModel
{
    public required string Review { get; set; }
    public RatingScore Score { get; set; }
}