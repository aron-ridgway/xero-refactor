using Testcontainers.MsSql;
using Testcontainers;

using Xunit;
using Xero.Api.Models;

public class SqlServerContainerFixture : IAsyncLifetime
{
    public MsSqlContainer SqlServerContainer { get; private set; }
    public string ConnectionString { get; private set; }
    public Product ExistingProduct {get; private set;}
    public ProductOption ExistingProductOption {get; private set;}

    public SqlServerContainerFixture()
    {
        var builder = new MsSqlBuilder()
        .WithName($"TestDatabase_{Guid.NewGuid()}");

        SqlServerContainer = builder.Build();
    }

    public async Task InitializeAsync()
    {
        // Start the container
        await SqlServerContainer.StartAsync();
        ConnectionString = SqlServerContainer.GetConnectionString(); 

        await ProductTableSetup.CreateProductTableAsync(ConnectionString);
        await ProductOptionTableSetup.CreateProductOptionTableAsync(ConnectionString);

        ExistingProduct = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            Description = "A test product",
            Price = 99.99m,
            DeliveryPrice = 9.99m
        };

        ExistingProductOption = new ProductOption
        {
            Id = Guid.NewGuid(),
            ProductId = ExistingProduct.Id,
            Name = "Test Product Option",
            Description = "A test product option"
        };

        await ProductTableSetup.InsertProductAsync(ConnectionString, ExistingProduct);
        await ProductOptionTableSetup.InsertProductOptionAsync(ConnectionString, ExistingProductOption);
    }

    public async Task DisposeAsync()
    {
        // Stop the container when the tests are done
        await SqlServerContainer.StopAsync();
    }
}

