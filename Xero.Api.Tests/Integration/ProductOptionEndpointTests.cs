using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xero.Api.Models;
using Xero.Api.Services;
using Xero.Api.Tests.Helpers;

namespace Xero.Api.Tests;
public class ProductOptionEndpointTests : IClassFixture<SqlServerContainerFixture>
{
    private readonly HttpClient _client;
    private readonly SqlServerContainerFixture _fixture;

    public ProductOptionEndpointTests(SqlServerContainerFixture fixture)
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
                    services.AddSingleton<IProductOptionService, ProductOptionService>();
                });
            });

        _client = webHostBuilder.CreateClient();
        _client.DefaultRequestHeaders.Add("X-Api-Key", "681212ab-9848-48a5-8723-f6cb0c34cbd7");
    }


    [Fact]
    public async Task GetAllProductOptions_ForProduct_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync($"api/products/{_fixture.ExistingProductOption.ProductId}/options");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var productOptions = await response.Content.ReadFromJsonAsync<IEnumerable<ProductOption>>();

        Assert.NotNull(productOptions);

        var productOption = productOptions.First(f => f.Id == _fixture.ExistingProductOption.Id);
        AssertProductOption(_fixture.ExistingProductOption, productOption);
    }

    [Fact]
    public async Task GetProductOptionById_ForProduct_ReturnsOk_WhenProductOptionExists()
    {
        // Arrange
        var optionId = _fixture.ExistingProductOption.Id;

        // Act
        var response = await _client.GetAsync($"api/products/{_fixture.ExistingProductOption.ProductId}/options/{optionId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var productOption = await response.Content.ReadFromJsonAsync<ProductOption>();

        Assert.NotNull(productOption);
        AssertProductOption(_fixture.ExistingProductOption, productOption);
    }

    [Fact]
    public async Task GetProductOptionById_ForProduct_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var nonExistentProductId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"api/products/{_fixture.ExistingProductOption.ProductId}/options/{nonExistentProductId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateProductOption_ForProduct_ReturnsCreated()
    {
        // Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),          
            Name = "Test Product For Option",
            Description = "A test product",
            Price = 99.99m,
            DeliveryPrice = 9.99m
        };
        await ProductTableSetup.InsertProductAsync(_fixture.ConnectionString, product);

        var newProductOption = new ProductOptionCreateRequest
        {
            ProductId = product.Id,
            Name = "New Product Option",
            Description = "New Description",
        };

        // Act
        var response = await _client.PostAsJsonAsync($"api/products/{newProductOption.ProductId}/options", newProductOption);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdResponse = await _client.GetAsync($"api/products/{newProductOption.ProductId}/options");
        var productOptions = await createdResponse.Content.ReadFromJsonAsync<IEnumerable<ProductOption>>();

        Assert.NotEmpty(productOptions);

        var createdProductOption = productOptions.First();

        Assert.Equal(newProductOption.Name, createdProductOption.Name);
        Assert.Equal(newProductOption.Description, createdProductOption.Description);
        Assert.Equal(newProductOption.ProductId, createdProductOption.ProductId);
    }

    [Fact]
    public async Task CreateProductOption_ProductDoesNotExist_ReturnsNotFoundResult()
    {
        var newProductOption = new ProductOptionCreateRequest
        {
            ProductId = _fixture.ExistingProduct.Id,
            Name = "New Product Option",
            Description = "New Description",
        };

        var badProductId = Guid.NewGuid();

        // Act
        var response = await _client.PostAsJsonAsync($"api/products/{badProductId}/options", newProductOption);
        var message = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains($"Product: {badProductId} does not exist", message);
    }

    [Fact]
    public async Task CreateProductOption_ForProduct_NullDescription_ReturnsBadResult()
    {
        // Arrange
        var newProductOption = new ProductOptionCreateRequest
        {
            Name = "New Product",
            Description = null,
        };

        // Act
        var response = await _client.PostAsJsonAsync($"api/products/{_fixture.ExistingProductOption.ProductId}/options", newProductOption);
        var validationMessage = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("The Description field is required.", validationMessage.Description.First());
    }

    [Fact]
    public async Task CreateProductOption_ForProduct_EmptyDescription_ReturnsBadResult()
    {
        // Arrange
        var newProductOption = new ProductOptionCreateRequest
        {
            Name = "New Product",
            Description = "",
        };

        // Act
        var response = await _client.PostAsJsonAsync($"api/products/{_fixture.ExistingProductOption.ProductId}/options", newProductOption);
        var validationMessage = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("The Description field is required.", validationMessage.Description.First());
    }

    [Fact]
    public async Task CreateProductOption_ForProduct_NullName_ReturnsBadResult()
    {
        // Arrange
        var newProductOption = new ProductOptionCreateRequest
        {
            Name = null,
            Description = "New Product",
        };

        // Act
        var response = await _client.PostAsJsonAsync($"api/products/{_fixture.ExistingProductOption.ProductId}/options", newProductOption);
        var validationMessage = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("The Name field is required.", validationMessage.Name.First());
    }

    [Fact]
    public async Task CreateProductOption_ForProduct_EmptyName_ReturnsBadResult()
    {
        // Arrange
        var newProductOption = new ProductOptionCreateRequest
        {
            Name = "",
            Description = "New Product",
        };

        // Act
        var response = await _client.PostAsJsonAsync($"api/products/{_fixture.ExistingProductOption.ProductId}/options", newProductOption);
        var validationMessage = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("The Name field is required.", validationMessage.Name.First());
    }

    [Fact]
    public async Task UpdateProductOption_ForProduct_ReturnsNoContent()
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

        var productOption = new ProductOption
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Name = "Test Product Option To Update",
            Description = "A test product option"
        };

        await ProductOptionTableSetup.InsertProductOptionAsync(_fixture.ConnectionString, productOption);
        var updateRequest = new ProductOptionUpdateRequest
        {
            Name = "Updated Product Option",
        };

        // Act
        var response = await _client.PutAsJsonAsync($"api/products/{product.Id}/options/{productOption.Id}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var updatedResponse = await _client.GetAsync($"api/products/{product.Id}/options/{productOption.Id}");
        var updatedProductOption = await updatedResponse.Content.ReadFromJsonAsync<ProductOption>();

        Assert.NotNull(updatedProductOption);

        Assert.Equal(updateRequest.Name, updatedProductOption.Name);
        //make sure original values have not changed
        Assert.Equal(productOption.Description, updatedProductOption.Description);
    }

    [Fact]
    public async Task DeleteProductOption_ForProduct_ReturnsNoContent()
    {
        // Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            Description = "A test product",
            Price = 99.99m,
            DeliveryPrice = 9.99m
        };

        await ProductTableSetup.InsertProductAsync(_fixture.ConnectionString, product);

        var productOption = new ProductOption
        {
            Id = Guid.NewGuid(),
            Name = "Test Product Option To Delete",
            Description = "A test product"
        };

        await ProductOptionTableSetup.InsertProductOptionAsync(_fixture.ConnectionString, productOption);

        // Act
        var response = await _client.DeleteAsync($"api/products/{product.Id}/options/{productOption.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var deletedResponse = await _client.GetAsync($"api/products/{product.Id}/options/{productOption.Id}");
        Assert.Equal(HttpStatusCode.NotFound, deletedResponse.StatusCode);
    }

    private void AssertProductOption(ProductOption expected, ProductOption actual)
    {
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.ProductId, actual.ProductId);
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Description, actual.Description);
    }
}
