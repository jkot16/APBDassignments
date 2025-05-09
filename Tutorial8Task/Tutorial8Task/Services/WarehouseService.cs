using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Tutorial8Task.Contracts.Requests;
using Tutorial8Task.Contracts.Responses;
using Tutorial8Task.Services;

namespace Tutorial8Task.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly string _cs;
        public WarehouseService(IConfiguration cfg)
            => _cs = cfg.GetConnectionString("DefaultConnection")!;

        public async Task<AddToWarehouseResponse> AddAsync(
            AddToWarehouseRequest req,
            CancellationToken cancellationToken)
        {
            const string sqlCheckProduct = @"
                SELECT COUNT(1)
                FROM Product
                WHERE IdProduct = @pid;";

            const string sqlCheckWarehouse = @"
                SELECT COUNT(1)
                FROM Warehouse
                WHERE IdWarehouse = @wid;";

            const string sqlOrder = @"
                SELECT TOP(1) IdOrder, CreatedAt, Amount
                FROM [Order]
                WHERE IdProduct = @pid 
                  AND Amount = @amt 
                  AND CreatedAt < @cat
                ORDER BY CreatedAt DESC;";

            const string sqlCheckFulfilled = @"
                SELECT COUNT(1)
                FROM Product_Warehouse
                WHERE IdOrder = @oid;";

            const string sqlUpdateOrder = @"
                UPDATE [Order]
                SET FulfilledAt = GETDATE()
                WHERE IdOrder = @oid;";

            const string sqlGetPrice = @"
                SELECT Price
                FROM Product
                WHERE IdProduct = @pid;";

            
            const string sqlInsert = @"
                INSERT INTO Product_Warehouse
                  (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)
                OUTPUT INSERTED.IdProductWarehouse
                VALUES
                  (@wid, @pid, @oid, @amt, @total, GETDATE());";

            await using var conn = new SqlConnection(_cs);
            await conn.OpenAsync(cancellationToken);
            await using var tx = (SqlTransaction)await conn.BeginTransactionAsync(cancellationToken);

            try
            {
                // Validate product exists
                await using (var cmd = new SqlCommand(sqlCheckProduct, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@pid", req.ProductId);
                    if ((int)await cmd.ExecuteScalarAsync(cancellationToken) != 1)
                        throw new KeyNotFoundException("Product does not exist.");
                }

                //Validate warehouse exists
                await using (var cmd = new SqlCommand(sqlCheckWarehouse, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@wid", req.WarehouseId);
                    if ((int)await cmd.ExecuteScalarAsync(cancellationToken) != 1)
                        throw new KeyNotFoundException("Warehouse does not exist.");
                }

                //Validate amount
                if (req.Amount <= 0)
                    throw new ArgumentException("Amount must be greater than zero.");

                //Fetch matching order
                int orderId;
                await using (var cmd = new SqlCommand(sqlOrder, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@pid", req.ProductId);
                    cmd.Parameters.AddWithValue("@amt", req.Amount);
                    cmd.Parameters.AddWithValue("@cat", req.CreatedAt);
                    await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                    if (!await reader.ReadAsync(cancellationToken))
                        throw new KeyNotFoundException("No matching order found.");
                    orderId = reader.GetInt32(reader.GetOrdinal("IdOrder"));
                }

                // Check not already fulfilled
                await using (var cmd = new SqlCommand(sqlCheckFulfilled, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@oid", orderId);
                    if ((int)await cmd.ExecuteScalarAsync(cancellationToken) > 0)
                        throw new InvalidOperationException("Order has already been fulfilled.");
                }

                // Mark order fulfilled
                await using (var cmd = new SqlCommand(sqlUpdateOrder, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@oid", orderId);
                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                }

                // Get unit price
                decimal unitPrice;
                await using (var cmd = new SqlCommand(sqlGetPrice, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@pid", req.ProductId);
                    unitPrice = (decimal)await cmd.ExecuteScalarAsync(cancellationToken);
                }

                // Insert into Product_Warehouse 
                await using (var cmd = new SqlCommand(sqlInsert, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@wid",   req.WarehouseId);
                    cmd.Parameters.AddWithValue("@pid",   req.ProductId);
                    cmd.Parameters.AddWithValue("@oid",   orderId);
                    cmd.Parameters.AddWithValue("@amt",   req.Amount);
                    cmd.Parameters.AddWithValue("@total", unitPrice * req.Amount);

                    var newId = (int)await cmd.ExecuteScalarAsync(cancellationToken);
                    await tx.CommitAsync(cancellationToken);
                    return new AddToWarehouseResponse { Id = newId };
                }
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<AddToWarehouseResponse> AddWithProcedureAsync(
            AddToWarehouseRequest req,
            CancellationToken cancellationToken)
        {
            const string procName = "AddProductToWarehouse";

            await using var conn = new SqlConnection(_cs);
            await conn.OpenAsync(cancellationToken);
            await using var cmd = new SqlCommand(procName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@IdProduct",   req.ProductId);
            cmd.Parameters.AddWithValue("@IdWarehouse", req.WarehouseId);
            cmd.Parameters.AddWithValue("@Amount",      req.Amount);
            cmd.Parameters.AddWithValue("@CreatedAt",   req.CreatedAt);

           
            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            var newId = Convert.ToInt32(result);
            return new AddToWarehouseResponse { Id = newId };
        }

    }
}
