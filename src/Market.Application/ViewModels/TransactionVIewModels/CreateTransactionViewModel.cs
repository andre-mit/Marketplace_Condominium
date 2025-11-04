using Market.Domain.Enums;

namespace Market.SharedApplication.ViewModels.TransactionVIewModels;

public class CreateTransactionViewModel
{
    public int ProductId { get; set; }
    public TransactionType TransactionType { get; set; }
}