using Market.Domain.Entities;

namespace Market.SharedApplication.ViewModels.CategoryViewModels;

public class ListCategoryViewModel
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public static implicit operator ListCategoryViewModel(Category category)
    {
        return new ListCategoryViewModel
        {
            Id = category.Id,
            Name = category.Name
        };
    }
}