using InventoryWpfApp.Models;
using InventoryWpfApp.Repositories.Contracts;
using InventoryWpfApp.Repositories.Helpers;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryWpfApp.Repositories.Implementations
{
    public class InventoryStockRepository : BaseRepository, IInventoryStockRepository
    {
        public InventoryStockRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }

        public IEnumerable<InventoryStock> GetAll()
        {
            var stockItems = new List<InventoryStock>();
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = "SELECT InventoryStockId, ProductId, SizeId, CurrentQuantity, MinStockLimit FROM InventoryStock";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stockItems.Add(new InventoryStock
                        {
                            InventoryStockId = reader.GetInt32(reader.GetOrdinal("InventoryStockId")),
                            ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                            SizeId = reader.GetInt32(reader.GetOrdinal("SizeId")),
                            CurrentQuantity = reader.GetInt32(reader.GetOrdinal("CurrentQuantity")),
                            MinStockLimit = reader.GetInt32(reader.GetOrdinal("MinStockLimit"))
                        });
                    }
                }
            }
            return stockItems;
        }

        public IEnumerable<InventoryStock> GetStockDetails()
        {
            var stockItems = new List<InventoryStock>();
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT
                        InvS.InventoryStockId,
                        InvS.ProductId,
                        P.Name AS ProductName,
                        InvS.SizeId,
                        S.SizeValue,
                        InvS.CurrentQuantity,
                        InvS.MinStockLimit
                    FROM InventoryStock InvS
                    JOIN Products P ON InvS.ProductId = P.ProductId
                    JOIN Sizes S ON InvS.SizeId = S.SizeId
                    ORDER BY P.Name, S.SizeValue";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stockItems.Add(new InventoryStock
                        {
                            InventoryStockId = reader.GetInt32(reader.GetOrdinal("InventoryStockId")),
                            ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                            ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                            SizeId = reader.GetInt32(reader.GetOrdinal("SizeId")),
                            SizeValue = reader.GetString(reader.GetOrdinal("SizeValue")),
                            CurrentQuantity = reader.GetInt32(reader.GetOrdinal("CurrentQuantity")),
                            MinStockLimit = reader.GetInt32(reader.GetOrdinal("MinStockLimit"))
                        });
                    }
                }
            }
            return stockItems;
        }

        public InventoryStock GetById(int id)
        {
            InventoryStock stock = null;
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = "SELECT InventoryStockId, ProductId, SizeId, CurrentQuantity, MinStockLimit FROM InventoryStock WHERE InventoryStockId = @InventoryStockId";
                command.Parameters.AddWithValue("@InventoryStockId", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        stock = new InventoryStock
                        {
                            InventoryStockId = reader.GetInt32(reader.GetOrdinal("InventoryStockId")),
                            ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                            SizeId = reader.GetInt32(reader.GetOrdinal("SizeId")),
                            CurrentQuantity = reader.GetInt32(reader.GetOrdinal("CurrentQuantity")),
                            MinStockLimit = reader.GetInt32(reader.GetOrdinal("MinStockLimit"))
                        };
                    }
                }
            }
            return stock;
        }

        public void Add(InventoryStock entity)
        {
            // Not used directly, prefer AddOrUpdateStock for initial stock
            throw new NotImplementedException("Use AddOrUpdateStock for initial inventory addition.");
        }

        public void Update(InventoryStock entity)
        {
            ExecuteNonQuery("UPDATE InventoryStock SET ProductId = @ProductId, SizeId = @SizeId, CurrentQuantity = @CurrentQuantity, MinStockLimit = @MinStockLimit WHERE InventoryStockId = @InventoryStockId", new[]
            {
                new SqlParameter("@ProductId", entity.ProductId),
                new SqlParameter("@SizeId", entity.SizeId),
                new SqlParameter("@CurrentQuantity", entity.CurrentQuantity),
                new SqlParameter("@MinStockLimit", entity.MinStockLimit),
                new SqlParameter("@InventoryStockId", entity.InventoryStockId)
            });
        }

        public void Delete(int id)
        {
            ExecuteNonQuery("DELETE FROM InventoryStock WHERE InventoryStockId = @InventoryStockId", new[]
            {
                new SqlParameter("@InventoryStockId", id)
            });
        }

        public void AddOrUpdateStock(int productId, int sizeId, int quantity, int minStockLimit)
        {
            string query = @"
                IF NOT EXISTS (SELECT 1 FROM InventoryStock WHERE ProductId = @ProductId AND SizeId = @SizeId)
                BEGIN
                    INSERT INTO InventoryStock (ProductId, SizeId, CurrentQuantity, MinStockLimit)
                    VALUES (@ProductId, @SizeId, @InitialQuantity, @MinStockLimit);
                END
                ELSE
                BEGIN
                    UPDATE InventoryStock
                    SET CurrentQuantity = CurrentQuantity + @InitialQuantity,
                        MinStockLimit = @MinStockLimit
                    WHERE ProductId = @ProductId AND SizeId = @SizeId;
                END";
            ExecuteNonQuery(query, new[]
            {
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@SizeId", sizeId),
                new SqlParameter("@InitialQuantity", quantity),
                new SqlParameter("@MinStockLimit", minStockLimit)
            });
        }

        public IEnumerable<InventoryStock> GetAvailableSizesForProduct(int productId)
        {
            var stockItems = new List<InventoryStock>();
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT
                        InvS.InventoryStockId,
                        S.SizeValue
                    FROM InventoryStock InvS
                    JOIN Sizes S ON InvS.SizeId = S.SizeId
                    WHERE InvS.ProductId = @ProductId AND InvS.CurrentQuantity > 0
                    ORDER BY S.SizeValue";
                command.Parameters.AddWithValue("@ProductId", productId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stockItems.Add(new InventoryStock
                        {
                            InventoryStockId = reader.GetInt32(reader.GetOrdinal("InventoryStockId")),
                            SizeValue = reader.GetString(reader.GetOrdinal("SizeValue"))
                        });
                    }
                }
            }
            return stockItems;
        }
    }
}
