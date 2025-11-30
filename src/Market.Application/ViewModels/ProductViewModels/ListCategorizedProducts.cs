using Market.SharedApplication.ViewModels.CategoryViewModels;

namespace Market.SharedApplication.ViewModels.ProductViewModels;

public class ListCategorizedProducts : ListCategoryViewModel
{
    public required List<ListProductViewModel> Products { get; set; }
}