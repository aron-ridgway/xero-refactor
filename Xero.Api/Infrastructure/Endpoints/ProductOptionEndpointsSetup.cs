using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MiniValidation;
using Xero.Api.Models;
using Xero.Api.Services;

namespace Xero.Api.Infrastucture.Endpoints;

public static class ProductOptionsEndpoints
{
    public static void MapProductOptionsEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("api/products/{productId}/options")
            .WithTags("ProductOptions")
            .WithMetadata(new ProducesResponseTypeAttribute(401))
            .WithMetadata(new ProducesResponseTypeAttribute(403));

        group.MapGet("", async (Guid productId, IProductOptionService productOptionService) => Results.Ok(await productOptionService.GetAllProductOptionsByProductIdAsync(productId)))
             .Produces<IEnumerable<ProductOption>>(200)
             .WithDescription("Get all product options for a specific product");

        group.MapGet("{optionId}", async (Guid productId, Guid optionId, IProductOptionService productOptionService) =>
        {
            var productOption = await productOptionService.GetProductOptionAsync(productId, optionId);
            return productOption is not null ? Results.Ok(productOption) : Results.NotFound();

        }).Produces<ProductOption>(200)
          .Produces(404)
          .WithDescription("Get a product option for a specific product");

        group.MapPost("", async (Guid productId, ProductOptionCreateRequest request, IProductOptionService productOptionService, IProductService productService) =>
        {
            request.ProductId = productId;

            if (!MiniValidator.TryValidate(request, out var errors))
                return Results.BadRequest(errors);
            
            if (!await productService.CheckProductExistsAsync(productId))
              return Results.NotFound($"Product: {productId} does not exist");

            await productOptionService.CreateProductOptionAsync(request);
            return Results.Created();

        }).Produces(201)
          .WithDescription("Create a new product option for a specific product");


        group.MapPut("{optionId}", async (Guid productId, Guid optionId, ProductOptionUpdateRequest request, IProductOptionService productOptionService) =>
        {           
            request.Id = optionId;
            request.ProductId = productId;

            await productOptionService.UpdateProductOptionAsync(request);
            return Results.NoContent();

        }).Produces(204)
          .WithDescription("Update a product option");

        group.MapDelete("{optionId}", async (Guid productId, Guid optionId, IProductOptionService productOptionService) =>
        {         
            await productOptionService.DeleteProductOptionAsync(productId, optionId);
            return Results.NoContent();

        }).Produces(204)
          .WithDescription("Delete a product option");
    }
}
