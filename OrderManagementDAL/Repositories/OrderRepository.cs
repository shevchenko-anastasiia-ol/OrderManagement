using System.Data;
using Dapper;
using MarketplaceDAL.Models;
using MarketplaceDAL.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace MarketplaceDAL.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbConnection _connection;

        public OrderRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        
        public async Task<Order> GetByIdempotencyTokenAsync(string idempotencyToken)
        {
            var sql = "SELECT * FROM Orders WHERE IdempotencyToken = @Token AND IsDeleted = 0";
            return await _connection.QueryFirstOrDefaultAsync<Order>(sql, new { Token = idempotencyToken });
        }

        public async Task AddAsync(Order entity, IDbTransaction? transaction = null)
        {
            var sql = @"INSERT INTO Orders 
                        (CustomerId, OrderDate, Status, TotalAmount, CreatedAt, CreatedBy, IsDeleted)
                        VALUES (@CustomerId, @OrderDate, @Status, @TotalAmount, @CreatedAt, @CreatedBy, @IsDeleted);
                        SELECT CAST(SCOPE_IDENTITY() as bigint);";

            entity.OrderId = await _connection.ExecuteScalarAsync<long>(sql, entity, transaction: transaction);
        }

        public async Task UpdateAsync(Order entity, IDbTransaction? transaction = null)
        {
            var sql = @"UPDATE Orders
                        SET CustomerId = @CustomerId,
                            OrderDate = @OrderDate,
                            Status = @Status,
                            TotalAmount = @TotalAmount,
                            UpdatedAt = @UpdatedAt,
                            UpdatedBy = @UpdatedBy
                        WHERE OrderId = @OrderId";

            await _connection.ExecuteAsync(sql, entity, transaction: transaction);
        }

        public async Task DeleteAsync(long id, IDbTransaction? transaction = null)
        {
            var sql = "UPDATE Orders SET IsDeleted = 1, UpdatedAt = @UpdatedAt WHERE OrderId = @Id";
            await _connection.ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.UtcNow }, transaction: transaction);
        }

        public async Task<IEnumerable<Order>> GetAllAsync(IDbTransaction? transaction = null)
        {
            var sql = "SELECT * FROM Orders WHERE IsDeleted = 0";
            return await _connection.QueryAsync<Order>(sql, transaction: transaction);
        }

        public async Task<Order?> GetByIdAsync(long id, IDbTransaction? transaction = null)
        {
            var sql = "SELECT * FROM Orders WHERE OrderId = @Id AND IsDeleted = 0";
            return await _connection.QuerySingleOrDefaultAsync<Order>(sql, new { Id = id }, transaction: transaction);
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(long customerId, IDbTransaction? transaction = null)
        {
            var sql = "SELECT * FROM Orders WHERE CustomerId = @CustomerId AND IsDeleted = 0";
            return await _connection.QueryAsync<Order>(sql, new { CustomerId = customerId }, transaction: transaction);
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status, IDbTransaction? transaction = null)
        {
            var sql = "SELECT * FROM Orders WHERE Status = @Status AND IsDeleted = 0";
            return await _connection.QueryAsync<Order>(sql, new { Status = status }, transaction: transaction);
        }

        public async Task<Order> GetOrderWithDetailsAsync(long orderId, IDbTransaction? transaction = null)
        {
            var sql = @"
                SELECT o.*, 
                       oi.OrderItemId, oi.ProductId, oi.Quantity, oi.UnitPrice,
                       p.PaymentId, p.Amount, p.PaymentDate, p.PaymentMethod, p.PaymentStatus,
                       s.ShipmentId, s.ShipmentDate, s.TrackingNumber, s.Carrier, s.Status, s.AddressLine1, s.AddressLine2, s.City, s.Region, s.PostalCode, s.Country
                FROM Orders o
                LEFT JOIN OrderItems oi ON o.OrderId = oi.OrderId
                LEFT JOIN Payments p ON o.OrderId = p.OrderId
                LEFT JOIN Shipments s ON o.OrderId = s.OrderId
                WHERE o.OrderId = @OrderId AND o.IsDeleted = 0";

            var orderDict = new Dictionary<long, Order>();

            var result = await _connection.QueryAsync<Order, OrderItem, Payment, Shipment, Order>(
                sql,
                (order, orderItem, payment, shipment) =>
                {
                    if (!orderDict.TryGetValue(order.OrderId, out var currentOrder))
                    {
                        currentOrder = order;
                        currentOrder.OrderItems = new List<OrderItem>();
                        currentOrder.Payments = new List<Payment>();
                        currentOrder.Shipments = new List<Shipment>();
                        orderDict.Add(currentOrder.OrderId, currentOrder);
                    }

                    if (orderItem != null && !currentOrder.OrderItems.Any(x => x.OrderItemId == orderItem.OrderItemId))
                        currentOrder.OrderItems.Add(orderItem);

                    if (payment != null && !currentOrder.Payments.Any(x => x.PaymentId == payment.PaymentId))
                        currentOrder.Payments.Add(payment);

                    if (shipment != null && !currentOrder.Shipments.Any(x => x.ShipmentId == shipment.ShipmentId))
                        currentOrder.Shipments.Add(shipment);

                    return currentOrder;
                },
                new { OrderId = orderId },
                transaction: transaction,
                splitOn: "OrderItemId,PaymentId,ShipmentId"
            );

            return orderDict.Values.FirstOrDefault()!;
        }
    }
}
