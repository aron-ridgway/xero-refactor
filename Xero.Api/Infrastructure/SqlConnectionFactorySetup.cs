using Xero.Api.Services;

namespace Xero.Api.Infrastructure;

public static class SqlConnectionFactorySetup
{
    public static IServiceCollection SetupSqlConnectionFactory(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ISqlConnectionFactory>(serviceProvider =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                throw new ApplicationException("Sql Connection string is null");

            return new SqlConnectionFactory(connectionString);
        });

        return services;
    }
}
