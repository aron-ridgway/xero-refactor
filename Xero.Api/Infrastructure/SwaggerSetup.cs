using System;
using Microsoft.OpenApi.Models;

namespace Xero.Api.Infrastructure;

public static class SwaggerSetup
{
    public static IServiceCollection SetupSwagger(this IServiceCollection services)
    {
        const string ApiKeyHeaderName = "X-Api-Key";
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Name = ApiKeyHeaderName,
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Description = "API Key needed to access endpoints.",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    }
                },
                Array.Empty<string>()
            }
            });
        });
        return services;
    }
}
