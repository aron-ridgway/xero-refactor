using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xero.Api.Models;
using Xero.Api.Services;

namespace Xero.Api.Tests;
public class ProductOptionEndpointAuthTests
{
    private readonly HttpClient _client;

    public ProductOptionEndpointAuthTests()
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
    public async Task GetAllProductOptions_ForProduct_NoApiKey_ReturnsUnauthorized()
    {   
        // Act
        var response = await _client.GetAsync($"api/products/{Guid.NewGuid()}/options");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task GetAllProductOptions_ForProduct_IncorrectApiKey_ReturnsForbidden()
    {   
        //Arrange
        _client.DefaultRequestHeaders.Add("X-Api-Key", "incorrect");

        // Act
        var response = await _client.GetAsync($"api/products/{Guid.NewGuid()}/options");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetProductOptionById_ForProduct_NoApiKey_ReturnsUnauthorized()
    {   
        // Act
        var response = await _client.GetAsync($"api/products/{Guid.NewGuid()}/options/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task GetProductOptionById_ForProduct_ReturnsForbidden()
    {   
        //Arrange
        _client.DefaultRequestHeaders.Add("X-Api-Key", "incorrect");

        // Act
        var response = await _client.GetAsync($"api/products/{Guid.NewGuid()}/options/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateProductOption_ForProduct_NoApiKey_ReturnsUnauthorized()
    {   
        // Act
        var response = await _client.PostAsJsonAsync($"api/products/{Guid.NewGuid()}/options", new ProductOption());

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateProductOption_ForProduct_IncorrectApiKey_ReturnsForbidden()
    {   
        //Arrange
        _client.DefaultRequestHeaders.Add("X-Api-Key", "incorrect");

        // Act
        var response = await _client.PostAsJsonAsync($"api/products/{Guid.NewGuid()}/options", new ProductOption());

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProductOption_ForProduct_NoApiKey_ReturnsUnauthorized()
    {   
        // Act
        var response = await _client.PutAsJsonAsync($"api/products/{Guid.NewGuid()}/options/{Guid.NewGuid()}", new ProductOptionUpdateRequest());

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateProductOption_ForProduct_IncorrectApiKey_ReturnsForbidden()
    {   
        //Arrange
        _client.DefaultRequestHeaders.Add("X-Api-Key", "incorrect");

        // Act
        var response = await _client.PutAsJsonAsync($"api/products/{Guid.NewGuid()}/options/{Guid.NewGuid()}", new ProductOptionUpdateRequest());
        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProductOption_ForProduct_NoApiKey_ReturnsUnauthorized()
    {   
        // Act
        var response = await _client.DeleteAsync($"api/products/{Guid.NewGuid()}/options/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteProductOption_ForProduct_IncorrectApiKey_ReturnsForbidden()
    {   
        //Arrange
        _client.DefaultRequestHeaders.Add("X-Api-Key", "incorrect");

        // Act
        var response = await _client.DeleteAsync($"api/products/{Guid.NewGuid()}/options/{Guid.NewGuid()}");
        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
