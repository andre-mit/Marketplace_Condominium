using Market.SharedApplication.ViewModels.CategoryViewModels;

namespace Market.SharedApplication.ViewModels.ProductViewModels;

public class ListCategorizedProductsViewModel : ListCategoryViewModel
{
    public required List<ListProductViewModel> Products { get; set; }
}