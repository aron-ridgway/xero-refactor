using Dapper;
using Microsoft.Data.SqlClient;
using Xero.Api.Models;

public static class ProductOptionTableSetup
{
    public static async Task CreateProductOptionTableAsync(string connectionString)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var createTableSql = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'ProductOption' AND xtype = 'U')
            BEGIN
                CREATE TABLE ProductOption (
                    Id UNIQUEIDENTIFIER PRIMARY KEY,
                    ProductId UNIQUEIDENTIFIER NOT NULL,
                    Name NVARCHAR(100) NOT NULL,
                    Description NVARCHAR(500) NOT NULL, 
                );
            END";

        await connection.ExecuteAsync(createTableSql);
    }

    public static async Task InsertProductOptionAsync(string connectionString, ProductOption productOption)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var insertSql = @"
            INSERT INTO ProductOption (Id, ProductId, Name, Description)
            VALUES (@Id, @ProductId, @Name, @Description)";

        await connection.ExecuteAsync(insertSql, productOption);
    }
}

