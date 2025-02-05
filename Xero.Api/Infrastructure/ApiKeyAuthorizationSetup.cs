
namespace Xero.Api.Infrastructure;

public static class ApiKeyAuthorizationSetup
{
    private const string ApiKeyHeaderName = "X-Api-Key";

    public static void UseApiKeyAuthorizationMiddleware(this IApplicationBuilder app, IConfiguration configuration)
    {
        app.Use(async (context, next) =>
        {
            var apiKey = configuration.GetValue<string>("ApiKey")!;
            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("API Key was not provided.");
                return;
            }

            if (!apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Unauthorized client.");
                return;
            }

            await next();
        });
    }
}
