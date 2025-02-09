using System.Data;
using Microsoft.Data.SqlClient;

namespace Xero.Api.Services;

public interface ISqlConnectionFactory
{
    IDbConnection Create();
}