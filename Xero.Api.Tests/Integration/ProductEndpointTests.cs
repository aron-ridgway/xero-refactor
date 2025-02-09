using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xero.Api.Models;
using Xero.Api.Services;
using Xero.Api.Tests.Helpers;

namespace Xero.Api.Tests;
public class ProductEndpointTests : IClassFixture<SqlServerContainerFixture>
{
    private readonly HttpClient _client;
    private readonly SqlServerContainerFixture _fixture;

    public ProductEndpointTests(SqlServerContainerFixture fixture)
    {
        _fixture = fixture;
        var webHostBuilder = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<ISqlConnectionFactory>(provider =>
                   {
                       return new SqlConnectionFactory(_fixture.ConnectionString);
                   });
                    services.AddSingleton<IProductService, ProductService>();
                });
            });

        _client = webHostBuilder.CreateClient();
        _client.DefaultRequestHeaders.Add("X-Api-Key", "681212ab-9848-48a5-8723-f6cb0c34cbd7");
    }


    [Fact]
    public async Task GetAllProducts_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("api/products");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var products = await response.Content.ReadFromJsonAsync<IEnumerable<Product>>();

        Assert.NotNull(products);

        var product = products.First(f => f.Id == _fixture.ExistingProduct.Id);
        AssertProduct(_fixture.ExistingProduct, product);
    }

    [Fact]
    public async Task GetProductById_ReturnsOk_WhenProductExists()
    {
        // Arrange
        var productId = _fixture.ExistingProduct.Id;

        // Act
        var response = await _client.GetAsync($"api/products/{productId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var product = await response.Content.ReadFromJsonAsync<Product>();

        Assert.NotNull(product);
        AssertProduct(_fixture.ExistingProduct, product);
    }

    [Fact]
    public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var nonExistentProductId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"api/products/{nonExistentProductId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SearchProductsByName_ReturnsOk_WhenProductsFound()
    {
        // Arrange
        var name = _fixture.ExistingProduct.Name;

        // Act
        var response = await _client.GetAsync($"api/products/search?name={name}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var products = await response.Content.ReadFromJsonAsync<IEnumerable<Product>>();

        Assert.NotEmpty(products);
        AssertProduct(_fixture.ExistingProduct, products.First());
    }

    [Fact]
    public async Task SearchProductsByName_ReturnsNotFound_WhenNoProductsFound()
    {
        // Arrange
        var name = "nonexistentProduct";

        // Act
        var response = await _client.GetAsync($"api/products/search?name={name}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_ReturnsCreated()
    {
        // Arrange
        var newProduct = new ProductCreateRequest
        {
            Name = "New Product",
            Description = "New Description",
            Price = 10.99m,
            DeliveryPrice = 5.99m
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/products", newProduct);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdResponse = await _client.GetAsync($"api/products/search?name={newProduct.Name}");
        var products = await createdResponse.Content.ReadFromJsonAsync<IEnumerable<Product>>();

        Assert.NotEmpty(products);

        var createdProduct = products.First();

        Assert.Equal(newProduct.Name, createdProduct.Name);
        Assert.Equal(newProduct.Description, createdProduct.Description);
        Assert.Equal(newProduct.Price, createdProduct.Price);
        Assert.Equal(newProduct.DeliveryPrice, createdProduct.DeliveryPrice);
    }

    [Fact]
    public async Task CreateProduct_NullDescription_ReturnsBadResult()
    {
        // Arrange
        var newProduct = new ProductCreateRequest
        {
            Name = "New Product",
            Description = null,
            Price = 10.99m,
            DeliveryPrice = 5.99m
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/products", newProduct);
        var validationMessage = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("The Description field is required.", validationMessage.Description.First());
    }

    [Fact]
    public async Task CreateProduct_EmptyDescription_ReturnsBadResult()
    {
        // Arrange
        var newProduct = new ProductCreateRequest
        {
            Name = "New Product",
            Description = "",
            Price = 10.99m,
            DeliveryPrice = 5.99m
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/products", newProduct);
        var validationMessage = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("The Description field is required.", validationMessage.Description.First());
    }

    [Fact]
    public async Task CreateProduct_NullName_ReturnsBadResult()
    {
        // Arrange
        var newProduct = new ProductCreateRequest
        {
            Name = null,
            Description = "New Product",
            Price = 10.99m,
            DeliveryPrice = 5.99m
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/products", newProduct);
        var validationMessage = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("The Name field is required.", validationMessage.Name.First());
    }

        [Fact]
    public async Task CreateProduct_EmptyName_ReturnsBadResult()
    {
        // Arrange
        var newProduct = new ProductCreateRequest
        {
            Name = "",
            Description = "New Product",
            Price = 10.99m,
            DeliveryPrice = 5.99m
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/products", newProduct);
        var validationMessage = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("The Name field is required.", validationMessage.Name.First());
    }

    [Fact]
    public async Task UpdateProduct_ReturnsNoContent()
    {
        // Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test Product To Update",
            Description = "A test product",
            Price = 99.99m,
            DeliveryPrice = 9.99m
        };

        await ProductTableSetup.InsertProductAsync(_fixture.ConnectionString, product);
        var updateRequest = new ProductUpdateRequest
        {
            Name = "Updated Product",
            Price = 15.99m
        };

        // Act
        var response = await _client.PutAsJsonAsync($"api/products/{product.Id}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var updatedResponse = await _client.GetAsync($"api/products/{product.Id}");
        var updatedProduct = await updatedResponse.Content.ReadFromJsonAsync<Product>();

        Assert.NotNull(updatedProduct);

        Assert.Equal(updateRequest.Name, updatedProduct.Name);
        Assert.Equal(updateRequest.Price, updatedProduct.Price);
        //make sure original values have not changed
        Assert.Equal(product.Description, updatedProduct.Description);
        Assert.Equal(product.DeliveryPrice, updatedProduct.DeliveryPrice);
    }

    [Fact]
    public async Task DeleteProduct_ReturnsNoContent()
    {
        // Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test Product To Delete",
            Description = "A test product",
            Price = 99.99m,
            DeliveryPrice = 9.99m
        };

        await ProductTableSetup.InsertProductAsync(_fixture.ConnectionString, product);

        // Act
        var response = await _client.DeleteAsync($"api/products/{product.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var deletedResponse = await _client.GetAsync($"api/products/{product.Id}");
        Assert.Equal(HttpStatusCode.NotFound, deletedResponse.StatusCode);
    }

    private void AssertProduct(Product expected, Product actual)
    {
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Description, actual.Description);
        Assert.Equal(expected.Price, actual.Price);
        Assert.Equal(expected.DeliveryPrice, actual.DeliveryPrice);
    }
}
