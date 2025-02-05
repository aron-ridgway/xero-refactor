using Xero.Api.Models;

namespace Xero.Api.Services;

public interface IProductOptionService
{
    Task<IEnumerable<ProductOption>> GetAllProductOptionsByProductIdAsync(Guid productId);
    Task<ProductOption?> GetProductOptionAsync(Guid productId, Guid optionId);
    Task CreateProductOptionAsync(ProductOptionCreateRequest request);
    Task UpdateProductOptionAsync(ProductOptionUpdateRequest request);
    Task DeleteProductOptionAsync(Guid productId, Guid optionIdid);
}