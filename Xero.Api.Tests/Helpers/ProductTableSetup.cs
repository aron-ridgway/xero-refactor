using Dapper;
using Microsoft.Data.SqlClient;
using Xero.Api.Models;

public static class ProductTableSetup
{
    public static async Task CreateProductTableAsync(string connectionString)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var createTableSql = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Product' AND xtype = 'U')
            BEGIN
                CREATE TABLE Product (
                    Id UNIQUEIDENTIFIER PRIMARY KEY,
                    Name NVARCHAR(100) NOT NULL,
                    Description NVARCHAR(500) NOT NULL,
                    Price DECIMAL(18, 2) NOT NULL,
                    DeliveryPrice DECIMAL(18, 2) NOT NULL,  
                );
            END";

        await connection.ExecuteAsync(createTableSql);
    }

    public static async Task InsertProductAsync(string connectionString, Product product)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var insertSql = @"
            INSERT INTO Product (Id, Name, Description, Price, DeliveryPrice)
            VALUES (@Id, @Name, @Description, @Price, @DeliveryPrice)";

        await connection.ExecuteAsync(insertSql, product);
    }
}

