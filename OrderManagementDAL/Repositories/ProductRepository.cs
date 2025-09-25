using System.Data;
using System.Data.SqlClient;
using Dapper;
using MarketplaceDAL.Models;
using MarketplaceDAL.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public class ProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    // для Dapper
    private IDbConnection GetConnection() => new SqlConnection(_connectionString);

    // --- Generic CRUD (Dapper) ---
    public async Task AddAsync(Product entity)
    {
        using var connection = GetConnection();
        var sql = @"INSERT INTO Products 
                    (ProductName, Description, Price, StockQuantity, CreatedAt, CreatedBy, IsDeleted)
                    VALUES (@ProductName, @Description, @Price, @StockQuantity, @CreatedAt, @CreatedBy, @IsDeleted);
                    SELECT CAST(SCOPE_IDENTITY() as bigint);";

        entity.ProductId = await connection.ExecuteScalarAsync<long>(sql, entity);
    }

    public async Task DeleteAsync(long id)
    {
        using var connection = GetConnection();
        var sql = "UPDATE Products SET IsDeleted = 1 WHERE ProductId = @Id";
        await connection.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        using var connection = GetConnection();
        var sql = "SELECT * FROM Products WHERE IsDeleted = 0";
        return await connection.QueryAsync<Product>(sql);
    }

    public async Task<Product> GetByIdAsync(long id)
    {
        using var connection = GetConnection();
        var sql = "SELECT * FROM Products WHERE ProductId = @Id AND IsDeleted = 0";
        return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
    }

    public async Task UpdateAsync(Product entity)
    {
        using var connection = GetConnection();
        var sql = @"UPDATE Products
                    SET ProductName = @ProductName,
                        Description = @Description,
                        Price = @Price,
                        StockQuantity = @StockQuantity,
                        UpdatedAt = @UpdatedAt,
                        UpdatedBy = @UpdatedBy
                    WHERE ProductId = @ProductId";

        await connection.ExecuteAsync(sql, entity);
    }

    // --- Спеціальні методи (Dapper) ---
    public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        using var connection = GetConnection();
        var sql = "SELECT * FROM Products WHERE Price BETWEEN @Min AND @Max AND IsDeleted = 0";
        return await connection.QueryAsync<Product>(sql, new { Min = minPrice, Max = maxPrice });
    }

    public async Task<IEnumerable<Product>> GetProductsInStockAsync()
    {
        using var connection = GetConnection();
        var sql = "SELECT * FROM Products WHERE StockQuantity > 0 AND IsDeleted = 0";
        return await connection.QueryAsync<Product>(sql);
    }

    public async Task<IEnumerable<Product>> FindProductsByNameAsync(string name)
    {
        using var connection = GetConnection();
        var sql = "SELECT * FROM Products WHERE ProductName LIKE @Name AND IsDeleted = 0";
        return await connection.QueryAsync<Product>(sql, new { Name = $"%{name}%" });
    }

    public async Task<IEnumerable<Product>> GetAvailableProductsAsync()
    {
        using var connection = GetConnection();
        var sql = "SELECT * FROM Products WHERE StockQuantity > 0 AND IsDeleted = 0";
        return await connection.QueryAsync<Product>(sql);
    }

    public async Task<Product> GetProductWithOrderItemsAsync(long productId)
    {
        using var connection = GetConnection();
        var sql = @"
        SELECT p.*, oi.OrderItemId, oi.OrderId, oi.Quantity, oi.UnitPrice
        FROM Products p
        LEFT JOIN OrderItems oi ON p.ProductId = oi.ProductId
        WHERE p.ProductId = @ProductId AND p.IsDeleted = 0";

        var productDict = new Dictionary<long, Product>();

        var result = await connection.QueryAsync<Product, OrderItem, Product>(
            sql,
            (product, orderItem) =>
            {
                if (!productDict.TryGetValue(product.ProductId, out var currentProduct))
                {
                    currentProduct = product;
                    currentProduct.OrderItems = new List<OrderItem>();
                    productDict.Add(product.ProductId, currentProduct);
                }

                if (orderItem != null)
                    currentProduct.OrderItems.Add(orderItem);

                return currentProduct;
            },
            new { ProductId = productId },
            splitOn: "OrderItemId"
        );

        return productDict.Values.FirstOrDefault();
    }

    // --- ТУТ ЧИСТИЙ ADO.NET ---
    // приклад 1: Порахувати кількість продуктів певного постачальника (чи будь-який критерій)
    public async Task<int> CountProductsInStockAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(
            "SELECT COUNT(*) FROM Products WHERE StockQuantity > 0 AND IsDeleted = 0",
            connection);

        var count = (int)await command.ExecuteScalarAsync();
        return count;
    }

    // приклад 2: Витягнути унікальні назви продуктів через DataReader
    public async Task<List<string>> GetDistinctProductNamesAsync()
    {
        var names = new List<string>();

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(
            "SELECT DISTINCT ProductName FROM Products WHERE IsDeleted = 0",
            connection);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            names.Add(reader.GetString(0));
        }

        return names;
    }
}
