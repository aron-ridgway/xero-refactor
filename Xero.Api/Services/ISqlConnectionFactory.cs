using Microsoft.Data.SqlClient;

namespace Xero.Api.Services;

public interface ISqlConnectionFactory
{
    SqlConnection Create();
}