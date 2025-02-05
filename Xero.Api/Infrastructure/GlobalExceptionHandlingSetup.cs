
namespace Xero.Api.Infrastructure;

public static class GlobalExceptionHandlingSetup
{
    public static void UseGlobalExceptionHandlingMiddleware(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(exceptionHandlerApp
            => exceptionHandlerApp.Run(async context
                => await Results.Problem()
                            .ExecuteAsync(context)));
    }
}
