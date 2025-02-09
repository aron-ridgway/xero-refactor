using Xero.Api.Infrastructure;
using Xero.Api.Infrastucture.Endpoints;
using Xero.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.SetupSwagger();
builder.Services.SetupSqlConnectionFactory(builder.Configuration);
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IProductOptionService, ProductOptionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//register endpoints
app.MapProductOptionsEndpoints();
app.MapProductEndpoints();

//register middleware
app.UseApiKeyAuthorizationMiddleware(builder.Configuration);
app.UseGlobalExceptionHandlingMiddleware();

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }

