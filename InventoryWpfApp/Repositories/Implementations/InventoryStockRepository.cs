using InventoryWpfApp.Models;
using InventoryWpfApp.Repositories.Contracts;
using InventoryWpfApp.Repositories.Helpers;
using Microsoft.Data.SqlClient;

namespace InventoryWpfApp.Repositories.Implementations
{
    /// <summary>
    /// Repository for managing inventory stock.
    /// </summary>
    public class InventoryStockRepository : BaseRepository, IInventoryStockRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryStockRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The database connection factory.</param>
        public InventoryStockRepository(IDbConnectionFactory connectionFactory)
            : base(connectionFactory) { }

        /// <summary>
        /// Gets all inventory stock items.
        /// </summary>
        /// <returns>A list of all inventory stock items.</returns>
        public IEnumerable<InventoryStock> GetAll()
        {
            var stockItems = new List<InventoryStock>();
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT InventoryStockId, ProductId, SizeId, CurrentQuantity, MinStockLimit FROM InventoryStock";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stockItems.Add(
                            new InventoryStock
                            {
                                InventoryStockId = reader.GetInt32(
                                    reader.GetOrdinal("InventoryStockId")
                                ),
                                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                SizeId = reader.GetInt32(reader.GetOrdinal("SizeId")),
                                CurrentQuantity = reader.GetInt32(
                                    reader.GetOrdinal("CurrentQuantity")
                                ),
                                MinStockLimit = reader.GetInt32(reader.GetOrdinal("MinStockLimit")),
                            }
                        );
                    }
                }
            }
            return stockItems;
        }

        /// <summary>
        /// Gets detailed information about the inventory stock items.
        /// </summary>
        /// <returns>A list of inventory stock items with product and size details.</returns>
        public IEnumerable<InventoryStock> GetStockDetails()
        {
            var stockItems = new List<InventoryStock>();
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText =
                    @"
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
                        stockItems.Add(
                            new InventoryStock
                            {
                                InventoryStockId = reader.GetInt32(
                                    reader.GetOrdinal("InventoryStockId")
                                ),
                                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                                SizeId = reader.GetInt32(reader.GetOrdinal("SizeId")),
                                SizeValue = reader.GetString(reader.GetOrdinal("SizeValue")),
                                CurrentQuantity = reader.GetInt32(
                                    reader.GetOrdinal("CurrentQuantity")
                                ),
                                MinStockLimit = reader.GetInt32(reader.GetOrdinal("MinStockLimit")),
                            }
                        );
                    }
                }
            }
            return stockItems;
        }

        /// <summary>
        /// Gets an inventory stock item by its ID.
        /// </summary>
        /// <param name="id">The ID of the inventory stock item.</param>
        /// <returns>The inventory stock item with the specified ID, or null if not found.</returns>
        public InventoryStock GetById(int id)
        {
            InventoryStock stock = null;
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT InventoryStockId, ProductId, SizeId, CurrentQuantity, MinStockLimit FROM InventoryStock WHERE InventoryStockId = @InventoryStockId";
                command.Parameters.AddWithValue("@InventoryStockId", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        stock = new InventoryStock
                        {
                            InventoryStockId = reader.GetInt32(
                                reader.GetOrdinal("InventoryStockId")
                            ),
                            ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                            SizeId = reader.GetInt32(reader.GetOrdinal("SizeId")),
                            CurrentQuantity = reader.GetInt32(reader.GetOrdinal("CurrentQuantity")),
                            MinStockLimit = reader.GetInt32(reader.GetOrdinal("MinStockLimit")),
                        };
                    }
                }
            }
            return stock;
        }

        /// <summary>
        /// Adds a new inventory stock item.
        /// </summary>
        /// <param name="entity">The inventory stock item to add.</param>
        public void Add(InventoryStock entity)
        {
            // Not used directly, prefer AddOrUpdateStock for initial stock
            throw new NotImplementedException(
                "Use AddOrUpdateStock for initial inventory addition."
            );
        }

        /// <summary>
        /// Updates an existing inventory stock item.
        /// </summary>
        /// <param name="entity">The inventory stock item to update.</param>
        public void Update(InventoryStock entity)
        {
            ExecuteNonQuery(
                "UPDATE InventoryStock SET ProductId = @ProductId, SizeId = @SizeId, CurrentQuantity = @CurrentQuantity, MinStockLimit = @MinStockLimit WHERE InventoryStockId = @InventoryStockId",
                new[]
                {
                    new SqlParameter("@ProductId", entity.ProductId),
                    new SqlParameter("@SizeId", entity.SizeId),
                    new SqlParameter("@CurrentQuantity", entity.CurrentQuantity),
                    new SqlParameter("@MinStockLimit", entity.MinStockLimit),
                    new SqlParameter("@InventoryStockId", entity.InventoryStockId),
                }
            );
        }

        /// <summary>
        /// Deletes an inventory stock item by its ID.
        /// </summary>
        /// <param name="id">The ID of the inventory stock item to delete.</param>
        public void Delete(int id)
        {
            ExecuteNonQuery(
                "DELETE FROM InventoryStock WHERE InventoryStockId = @InventoryStockId",
                new[] { new SqlParameter("@InventoryStockId", id) }
            );
        }

        /// <summary>
        /// Adds or updates stock for a product and size.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <param name="sizeId">The ID of the size.</param>
        /// <param name="quantity">The quantity to add.</param>
        /// <param name="minStockLimit">The minimum stock limit.</param>
        public void AddOrUpdateStock(int productId, int sizeId, int quantity, int minStockLimit)
        {
            string query =
                @"
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
            ExecuteNonQuery(
                query,
                new[]
                {
                    new SqlParameter("@ProductId", productId),
                    new SqlParameter("@SizeId", sizeId),
                    new SqlParameter("@InitialQuantity", quantity),
                    new SqlParameter("@MinStockLimit", minStockLimit),
                }
            );
        }

        /// <summary>
        /// Gets all available sizes for a product.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <returns>A list of available inventory stock items for the specified product.</returns>
        public IEnumerable<InventoryStock> GetAvailableSizesForProduct(int productId)
        {
            var stockItems = new List<InventoryStock>();
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText =
                    @"
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
                        stockItems.Add(
                            new InventoryStock
                            {
                                InventoryStockId = reader.GetInt32(
                                    reader.GetOrdinal("InventoryStockId")
                                ),
                                SizeValue = reader.GetString(reader.GetOrdinal("SizeValue")),
                            }
                        );
                    }
                }
            }
            return stockItems;
        }
    }
}
