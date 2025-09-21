using System.Linq.Expressions;
using Domain.Entities;

namespace Domain.Queries;

public static class ProductQueries
{
    public static Expression<Func<Product, bool>> GetByOwnerId(Guid ownerId)
    {
        return product => product.Owner.Id == ownerId;
    }
    
    public static Expression<Func<Product, bool>> GetAvailableProducts()
    {
        return product => product.IsAvailable;
    }
    
    public static Expression<Func<Product, bool>> GetById(int productId)
    {
        return product => product.Id == productId;
    }
    
    public static Expression<Func<Product, bool>> SearchByNameOrDescription(string searchTerm)
    {
        return product => product.Name.Contains(searchTerm) || product.Description.Contains(searchTerm);
    }
    
    public static Expression<Func<Product, bool>> GetByAdvertisementType(params Domain.Enums.ProductAdvertisementType[] types)
    {
        return product => product.AdvertisementTypes.Any(type => types.Contains(type));
    }
    
    public static Expression<Func<Product, bool>> GetByPriceRange(decimal minPrice, decimal maxPrice)
    {
        return product => product.Price >= minPrice && product.Price <= maxPrice;
    }
}