using System.Data;
using MarketplaceDAL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace MarketplaceDAL.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly string _connectionString;

    public CustomerRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    // --- Generic CRUD через чистий ADO.NET ---
    public async Task AddAsync(Customer entity)
    {
        using var conn = new SqlConnection(_connectionString);
        var sql = @"INSERT INTO Customers (FirstName, LastName, Email, Phone, CreatedAt, CreatedBy, IsDeleted)
                    VALUES (@FirstName, @LastName, @Email, @Phone, @CreatedAt, @CreatedBy, @IsDeleted);
                    SELECT CAST(SCOPE_IDENTITY() as bigint);";

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@FirstName", entity.FirstName);
        cmd.Parameters.AddWithValue("@LastName", entity.LastName);
        cmd.Parameters.AddWithValue("@Email", entity.Email);
        cmd.Parameters.AddWithValue("@Phone", entity.Phone);
        cmd.Parameters.AddWithValue("@CreatedAt", entity.CreatedAt);
        cmd.Parameters.AddWithValue("@CreatedBy", entity.CreatedBy);
        cmd.Parameters.AddWithValue("@IsDeleted", entity.IsDeleted);

        await conn.OpenAsync();
        entity.CustomerId = Convert.ToInt64(await cmd.ExecuteScalarAsync());
    }

    public async Task DeleteAsync(long id)
    {
        using var conn = new SqlConnection(_connectionString);
        var sql = "UPDATE Customers SET IsDeleted = 1 WHERE CustomerId = @Id";

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", id);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        var list = new List<Customer>();
        using var conn = new SqlConnection(_connectionString);
        var sql = "SELECT * FROM Customers WHERE IsDeleted = 0";

        using var cmd = new SqlCommand(sql, conn);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var customer = new Customer
            {
                CustomerId = reader.GetInt64(reader.GetOrdinal("CustomerId")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted"))
            };
            list.Add(customer);
        }

        return list;
    }

    public async Task<Customer> GetByIdAsync(long id)
    {
        using var conn = new SqlConnection(_connectionString);
        var sql = "SELECT * FROM Customers WHERE CustomerId = @Id AND IsDeleted = 0";

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", id);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Customer
            {
                CustomerId = reader.GetInt64(reader.GetOrdinal("CustomerId")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted"))
            };
        }

        return null;
    }

    public async Task UpdateAsync(Customer entity)
    {
        using var conn = new SqlConnection(_connectionString);
        var sql = @"UPDATE Customers
                    SET FirstName = @FirstName,
                        LastName = @LastName,
                        Email = @Email,
                        Phone = @Phone,
                        UpdatedAt = @UpdatedAt,
                        UpdatedBy = @UpdatedBy
                    WHERE CustomerId = @CustomerId";

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@FirstName", entity.FirstName);
        cmd.Parameters.AddWithValue("@LastName", entity.LastName);
        cmd.Parameters.AddWithValue("@Email", entity.Email);
        cmd.Parameters.AddWithValue("@Phone", entity.Phone);
        cmd.Parameters.AddWithValue("@UpdatedAt", entity.UpdatedAt ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@UpdatedBy", entity.UpdatedBy ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@CustomerId", entity.CustomerId);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    // --- Спеціальні методи ---

    public async Task<IEnumerable<Customer>> GetCustomersCreatedAfterAsync(DateTime date)
    {
        var list = new List<Customer>();
        using var conn = new SqlConnection(_connectionString);
        var sql = "SELECT * FROM Customers WHERE CreatedAt > @Date AND IsDeleted = 0";

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Date", date);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var customer = new Customer
            {
                CustomerId = reader.GetInt64(reader.GetOrdinal("CustomerId")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted"))
            };
            list.Add(customer);
        }

        return list;
    }

    public async Task<Customer> GetCustomerWithOrdersAsync(long customerId)
    {
        Customer customer = null;
        using var conn = new SqlConnection(_connectionString);
        var sql = @"
            SELECT c.*, o.OrderId, o.CustomerId AS OrderCustomerId, o.OrderDate, o.Status, o.TotalAmount
            FROM Customers c
            LEFT JOIN Orders o ON c.CustomerId = o.CustomerId
            WHERE c.CustomerId = @CustomerId AND c.IsDeleted = 0";

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@CustomerId", customerId);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            if (customer == null)
            {
                customer = new Customer
                {
                    CustomerId = reader.GetInt64(reader.GetOrdinal("CustomerId")),
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                    IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
                    Orders = new List<Order>()
                };
            }

            if (!reader.IsDBNull(reader.GetOrdinal("OrderId")))
            {
                var order = new Order
                {
                    OrderId = reader.GetInt64(reader.GetOrdinal("OrderId")),
                    CustomerId = reader.GetInt64(reader.GetOrdinal("OrderCustomerId")),
                    OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount"))
                };
                customer.Orders.Add(order);
            }
        }
        return customer;
    }
}
