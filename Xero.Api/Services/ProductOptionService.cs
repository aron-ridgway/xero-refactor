using Dapper;
using Xero.Api.Models;

namespace Xero.Api.Services;

public class ProductOptionService : IProductOptionService
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly ILogger<ProductOptionService> _logger;

    public ProductOptionService(ISqlConnectionFactory sqlConnectionFactory, ILogger<ProductOptionService> logger)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _logger = logger;
    }

    public async Task CreateProductOptionAsync(ProductOptionCreateRequest request)
    {
        try
        {
            using var connection = _sqlConnectionFactory.Create();

            request.Id = Guid.NewGuid();

            const string sql = """
                INSERT INTO ProductOption (Id, ProductId, Name, Description)
                VALUES (@Id, @ProductId, @Name, @Description)
            """;

            await connection.ExecuteAsync(sql, request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateProductOptionAsync: {Message}", ex.Message);
            throw;
        }
    }

        public async Task UpdateProductOptionAsync(ProductOptionUpdateRequest request)
    {
        try
        {
            using var connection = _sqlConnectionFactory.Create();

            const string sql = """
                UPDATE ProductOption
                SET Name = COALESCE(@Name, Name), 
                    Description = COALESCE(@Description, Description) 
                WHERE Id = @Id AND ProductId = @ProductId
            """;

            await connection.ExecuteAsync(sql, request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateProductOptionAsync: {Message}", ex.Message);
            throw;
        }
    }

    public async Task DeleteProductOptionAsync(Guid productId, Guid optionId)
    {
        try
        {
            using var connection = _sqlConnectionFactory.Create();

            const string sql = "DELETE FROM ProductOption WHERE Id = @ProductOptionId AND ProductId = @ProductId";

            await connection.ExecuteAsync(sql, new { ProductOptionId = optionId, ProductId = productId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteProductOptionAsync: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<IEnumerable<ProductOption>> GetAllProductOptionsByProductIdAsync(Guid productId)
    {
        try
        {
            using var connection = _sqlConnectionFactory.Create();

            const string sql = """
                SELECT Id, ProductId, Name, Description 
                FROM ProductOption
                WHERE ProductId = @ProductId
            """;

            return await connection.QueryAsync<ProductOption>(
                sql,
                new {ProductId = productId}
                );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllProductOptionsByProductIdAsync: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<ProductOption?> GetProductOptionAsync(Guid productId, Guid productOptionId)
    {
        try
        {
            using var connection = _sqlConnectionFactory.Create();

            const string sql = """
                SELECT Id, ProductId, Name, Description
                FROM ProductOption
                WHERE Id = @ProductOptionId and ProductId = @ProductId
            """;

            return await connection.QuerySingleOrDefaultAsync<ProductOption>(
                sql,
                new { ProductId = productId, ProductOptionId = productOptionId }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetProductOptionAsync: {Message}", ex.Message);
            throw;
        }
    }
}
