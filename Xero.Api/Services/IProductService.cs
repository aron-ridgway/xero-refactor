using Xero.Api.Models;

namespace Xero.Api.Services;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(Guid id);
    Task<IEnumerable<Product>> GetProductsByNameAsync(string name);
    Task CreateProductAsync(ProductCreateRequest request);
    Task UpdateProductAsync(ProductUpdateRequest request);
    Task DeleteProductAsync(Guid id);
    Task<bool> CheckProductExistsAsync(Guid id);
}