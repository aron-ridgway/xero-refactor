using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MiniValidation;
using Xero.Api.Models;
using Xero.Api.Services;

namespace Xero.Api.Infrastucture.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("api/products")
            .WithTags("Products")
            .WithMetadata(new ProducesResponseTypeAttribute(401))
            .WithMetadata(new ProducesResponseTypeAttribute(403));

        group.MapGet("", async (IProductService productService) => Results.Ok(await productService.GetAllProductsAsync()))
             .Produces<IEnumerable<Product>>(200)
             .WithDescription("Get all products")
             .WithName("GetAllProducts");

        group.MapGet("{productId}", async (Guid productId, IProductService productService) =>
        {
            var product = await productService.GetProductByIdAsync(productId);
            return product is not null ? Results.Ok(product) : Results.NotFound();

        }).Produces<Product>(200)
          .Produces(404)
          .WithDescription("Get a product by id");

        group.MapGet("search", async ([Required][StringLength(100)] string name, IProductService productService) =>
        {
            var products = await productService.GetProductsByNameAsync(name);
            return products.Any() ? Results.Ok(products) : Results.NotFound();

        }).Produces<Product>(200)
          .Produces(404)
          .WithDescription("Search all products by name");

        group.MapPost("", async (ProductCreateRequest request, IProductService productService) =>
        {
            if (!MiniValidator.TryValidate(request, out var errors))
                return Results.BadRequest(errors);
            
            await productService.CreateProductAsync(request);
            return Results.Created();

        }).Produces(201)
          .Produces(400)
          .WithDescription("Create a new product");

        group.MapPut("{productId}", async (Guid productId, ProductUpdateRequest request, IProductService productService) =>
        {
            request.Id = productId;
            await productService.UpdateProductAsync(request);
            return Results.NoContent();

        }).Produces(204)
          .WithDescription("Update a product");

        group.MapDelete("{productId}", async (Guid productId, IProductService productService) =>
        {         
            await productService.DeleteProductAsync(productId);
            return Results.NoContent();

        }).Produces(204)
          .WithDescription("Delete a product");
    }
}
