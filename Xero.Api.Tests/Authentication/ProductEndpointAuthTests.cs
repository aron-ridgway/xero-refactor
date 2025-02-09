using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xero.Api.Models;
using Xero.Api.Services;

namespace Xero.Api.Tests;
public class ProductEndpointAuthTests
{
    private readonly HttpClient _client;

    public ProductEndpointAuthTests()
    {
        var webHostBuilder = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IProductService, ProductService>();
                });
            });

        _client = webHostBuilder.CreateClient();
    }

    [Fact]
    public async Task GetAllProducts_NoApiKey_ReturnsUnauthorized()
    {   
        // Act
        var response = await _client.GetAsync("api/products");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task GetAllProducts_IncorrectApiKey_ReturnsForbidden()
    {   
        //Arrange
        _client.DefaultRequestHeaders.Add("X-Api-Key", "incorrect");

        // Act
        var response = await _client.GetAsync("api/products");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetProductById_NoApiKey_ReturnsUnauthorized()
    {   
        // Act
        var response = await _client.GetAsync($"api/products/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task GetProductById_IncorrectApiKey_ReturnsForbidden()
    {   
        //Arrange
        _client.DefaultRequestHeaders.Add("X-Api-Key", "incorrect");

        // Act
        var response = await _client.GetAsync($"api/products/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task SearchProductsByName_NoApiKey_ReturnsUnauthorized()
    {   
        // Act
        var response = await _client.GetAsync($"api/products/search?name={string.Empty}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task SearchProductsByName_IncorrectApiKey_ReturnsForbidden()
    {   
        //Arrange
        _client.DefaultRequestHeaders.Add("X-Api-Key", "incorrect");

        // Act
        var response = await _client.GetAsync($"api/products/search?name={string.Empty}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_NoApiKey_ReturnsUnauthorized()
    {   
        // Act
        var response = await _client.PostAsJsonAsync("api/products", new Product());

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateProduct_IncorrectApiKey_ReturnsForbidden()
    {   
        //Arrange
        _client.DefaultRequestHeaders.Add("X-Api-Key", "incorrect");

        // Act
        var response = await _client.PostAsJsonAsync("api/products", new Product());

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_NoApiKey_ReturnsUnauthorized()
    {   
        // Act
        var response = await _client.PutAsJsonAsync($"api/products/{Guid.NewGuid()}", new ProductUpdateRequest());

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateProduct_IncorrectApiKey_ReturnsForbidden()
    {   
        //Arrange
        _client.DefaultRequestHeaders.Add("X-Api-Key", "incorrect");

        // Act
        var response = await _client.PutAsJsonAsync($"api/products/{Guid.NewGuid()}", new ProductUpdateRequest());
        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_NoApiKey_ReturnsUnauthorized()
    {   
        // Act
        var response = await _client.DeleteAsync($"api/products/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteProduct_IncorrectApiKey_ReturnsForbidden()
    {   
        //Arrange
        _client.DefaultRequestHeaders.Add("X-Api-Key", "incorrect");

        // Act
        var response = await _client.DeleteAsync($"api/products/{Guid.NewGuid()}");
        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
