using System.Data;
using Dapper;
using MarketplaceDAL.Models;
using MarketplaceDAL.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketplaceDAL.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDbConnection _connection;

        public ProductRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task AddAsync(Product entity, IDbTransaction? transaction = null)
        {
            var sql = @"INSERT INTO Products 
                        (ProductName, Description, Price, StockQuantity, CreatedAt, CreatedBy, IsDeleted)
                        VALUES (@ProductName, @Description, @Price, @StockQuantity, @CreatedAt, @CreatedBy, @IsDeleted);
                        SELECT CAST(SCOPE_IDENTITY() as bigint);";

            entity.ProductId = await _connection.ExecuteScalarAsync<long>(sql, entity, transaction: transaction);
        }

        public async Task UpdateAsync(Product entity, IDbTransaction? transaction = null)
        {
            var sql = @"UPDATE Products
                        SET ProductName = @ProductName,
                            Description = @Description,
                            Price = @Price,
                            StockQuantity = @StockQuantity,
                            UpdatedAt = @UpdatedAt,
                            UpdatedBy = @UpdatedBy
                        WHERE ProductId = @ProductId";

            await _connection.ExecuteAsync(sql, entity, transaction: transaction);
        }

        public async Task DeleteAsync(long id, IDbTransaction? transaction = null)
        {
            var sql = "UPDATE Products SET IsDeleted = 1, UpdatedAt = @UpdatedAt WHERE ProductId = @Id";
            await _connection.ExecuteAsync(sql, new { Id = id, UpdatedAt = System.DateTime.UtcNow }, transaction: transaction);
        }

        public async Task<IEnumerable<Product>> GetAllAsync(IDbTransaction? transaction = null)
        {
            var sql = "SELECT * FROM Products WHERE IsDeleted = 0";
            return await _connection.QueryAsync<Product>(sql, transaction: transaction);
        }

        public async Task<Product?> GetByIdAsync(long id, IDbTransaction? transaction = null)
        {
            var sql = "SELECT * FROM Products WHERE ProductId = @Id AND IsDeleted = 0";
            return await _connection.QuerySingleOrDefaultAsync<Product>(sql, new { Id = id }, transaction: transaction);
        }

        public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice, IDbTransaction? transaction = null)
        {
            var sql = "SELECT * FROM Products WHERE Price BETWEEN @Min AND @Max AND IsDeleted = 0";
            return await _connection.QueryAsync<Product>(sql, new { Min = minPrice, Max = maxPrice }, transaction: transaction);
        }

        public async Task<IEnumerable<Product>> GetProductsInStockAsync(IDbTransaction? transaction = null)
        {
            var sql = "SELECT * FROM Products WHERE StockQuantity > 0 AND IsDeleted = 0";
            return await _connection.QueryAsync<Product>(sql, transaction: transaction);
        }

        public async Task<IEnumerable<Product>> FindProductsByNameAsync(string name, IDbTransaction? transaction = null)
        {
            var sql = "SELECT * FROM Products WHERE ProductName LIKE @Name AND IsDeleted = 0";
            return await _connection.QueryAsync<Product>(sql, new { Name = $"%{name}%" }, transaction: transaction);
        }

        public async Task<IEnumerable<Product>> GetAvailableProductsAsync(IDbTransaction? transaction = null)
        {
            var sql = "SELECT * FROM Products WHERE StockQuantity > 0 AND IsDeleted = 0";
            return await _connection.QueryAsync<Product>(sql, transaction: transaction);
        }

        public async Task<Product?> GetProductWithOrderItemsAsync(long productId, IDbTransaction? transaction = null)
        {
            var sql = @"
                SELECT p.*, oi.OrderItemId, oi.OrderId, oi.Quantity, oi.UnitPrice
                FROM Products p
                LEFT JOIN OrderItems oi ON p.ProductId = oi.ProductId
                WHERE p.ProductId = @ProductId AND p.IsDeleted = 0";

            var productDict = new Dictionary<long, Product>();

            var result = await _connection.QueryAsync<Product, OrderItem, Product>(
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
                splitOn: "OrderItemId",
                transaction: transaction
            );

            return productDict.Values.FirstOrDefault();
        }

        public async Task<Product> GetByIdempotencyTokenAsync(string idempotencyToken)
        {
            var sql = "SELECT * FROM Products WHERE IdempotencyToken = @Token AND IsDeleted = 0";
            return await _connection.QueryFirstOrDefaultAsync<Product>(sql, new { Token = idempotencyToken });
        }
        public async Task<int> CountProductsInStockAsync(IDbTransaction? transaction = null)
        {
            var sql = "SELECT COUNT(*) FROM Products WHERE StockQuantity > 0 AND IsDeleted = 0";
            return await _connection.ExecuteScalarAsync<int>(sql, transaction: transaction);
        }

        public async Task<List<string>> GetDistinctProductNamesAsync(IDbTransaction? transaction = null)
        {
            var sql = "SELECT DISTINCT ProductName FROM Products WHERE IsDeleted = 0";
            var result = await _connection.QueryAsync<string>(sql, transaction: transaction);
            return result.ToList();
        }
    }
}
