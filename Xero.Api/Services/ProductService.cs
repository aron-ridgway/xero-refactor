using Dapper;
using Xero.Api.Models;

namespace Xero.Api.Services;

public class ProductService : IProductService
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly ILogger<ProductService> _logger;

    public ProductService(ISqlConnectionFactory sqlConnectionFactory, ILogger<ProductService> logger)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _logger = logger;
    }

    public async Task CreateProductAsync(ProductCreateRequest request)
    {
        try
        {
            using var connection = _sqlConnectionFactory.Create();

            request.Id = Guid.NewGuid();

            const string sql = """
                INSERT INTO Product (Id, Name, Description, Price, DeliveryPrice)
                VALUES (@Id, @Name, @Description, @Price, @DeliveryPrice )
            """;

            await connection.ExecuteAsync(sql, request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateProductAsync: {Message}", ex.Message);
            throw;
        }
    }

    public async Task UpdateProductAsync(ProductUpdateRequest request)
    {
        try
        {
            using var connection = _sqlConnectionFactory.Create();

            const string sql = """
                UPDATE Product
                SET Name = COALESCE(@Name, Name), 
                    Description = COALESCE(@Description, Description), 
                    Price = COALESCE(@Price, Price), 
                    DeliveryPrice = COALESCE(@DeliveryPrice, DeliveryPrice)
                WHERE Id = @Id
            """;

            await connection.ExecuteAsync(sql, request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateProductAsync: {Message}", ex.Message);
            throw;
        }
    }

    public async Task DeleteProductAsync(Guid id)
    {
        try
        {
            using var connection = _sqlConnectionFactory.Create();

            const string sql = "DELETE FROM Product WHERE Id = @ProductId";

            await connection.ExecuteAsync(sql, new { ProductId = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteProductAsync: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        try
        {
            using var connection = _sqlConnectionFactory.Create();

            const string sql = "SELECT Id, Name, Description, Price, DeliveryPrice FROM Product";

            return await connection.QueryAsync<Product>(sql);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllProductsAsync: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<Product?> GetProductByIdAsync(Guid id)
    {
        try
        {
            using var connection = _sqlConnectionFactory.Create();

            const string sql = """
            SELECT Id, Name, Description, Price, DeliveryPrice 
            FROM Product
            WHERE Id = @ProductId
            """;

            return await connection.QuerySingleOrDefaultAsync<Product>(
                sql,
                new { ProductId = id }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetProductByIdAsync: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
    {
        try
        {
            using var connection = _sqlConnectionFactory.Create();

            const string sql = """
                SELECT Id, Name, Description, Price, DeliveryPrice
                FROM Product
                WHERE Name LIKE @ProductName
            """;

            return await connection.QueryAsync<Product>(
                sql,
                new { ProductName = $"%{name}%" }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetProductsByNameAsync: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<bool> CheckProductExistsAsync(Guid id)
    {
         try
        {
            using var connection = _sqlConnectionFactory.Create();

            const string sql = """
                SELECT 1
                FROM Product
                WHERE Id = @ProductId
            """;

            return await connection.ExecuteScalarAsync<bool>(
                sql,
                new { ProductId = id }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CheckProductExistsAsync: {Message}", ex.Message);
            throw;
        }
    }
}
