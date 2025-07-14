using InventoryWpfApp.Models;
using InventoryWpfApp.Repositories.Contracts;
using InventoryWpfApp.Repositories.Helpers;
using Microsoft.Data.SqlClient;

namespace InventoryWpfApp.Repositories.Implementations
{
    /// <summary>
    /// Repository for managing products in the inventory.
    /// </summary>
    public class ProductRepository : BaseRepository, IProductRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The database connection factory.</param>
        public ProductRepository(IDbConnectionFactory connectionFactory)
            : base(connectionFactory) { }

        /// <summary>
        /// Gets all products in the inventory.
        /// </summary>
        /// <returns>A list of all products in the inventory.</returns>
        public IEnumerable<Product> GetAll()
        {
            var products = new List<Product>();
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT ProductId, Name, Description, ApplicabilityType FROM Products";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(
                            new Product
                            {
                                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("Description")),
                                ApplicabilityType = reader.GetString(
                                    reader.GetOrdinal("ApplicabilityType")
                                ),
                            }
                        );
                    }
                }
            }
            return products;
        }

        /// <summary>
        /// Gets a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <returns>The product with the specified ID, or null if not found.</returns>
        public Product GetById(int id)
        {
            Product product = null;
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT ProductId, Name, Description, ApplicabilityType FROM Products WHERE ProductId = @ProductId";
                command.Parameters.AddWithValue("@ProductId", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        product = new Product
                        {
                            ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("Description")),
                            ApplicabilityType = reader.GetString(
                                reader.GetOrdinal("ApplicabilityType")
                            ),
                        };
                    }
                }
            }
            return product;
        }

        /// <summary>
        /// Adds a new product to the inventory.
        /// </summary>
        /// <param name="product">The product to add.</param>
        public void Add(Product product)
        {
            ExecuteNonQuery(
                "INSERT INTO Products (Name, Description, ApplicabilityType) VALUES (@Name, @Description, @ApplicabilityType)",
                new[]
                {
                    new SqlParameter("@Name", product.Name),
                    new SqlParameter("@Description", (object)product.Description ?? DBNull.Value),
                    new SqlParameter("@ApplicabilityType", product.ApplicabilityType),
                }
            );
        }

        /// <summary>
        /// Updates an existing product in the inventory.
        /// </summary>
        /// <param name="product">The product to update.</param>
        public void Update(Product product)
        {
            ExecuteNonQuery(
                "UPDATE Products SET Name = @Name, Description = @Description, ApplicabilityType = @ApplicabilityType WHERE ProductId = @ProductId",
                new[]
                {
                    new SqlParameter("@Name", product.Name),
                    new SqlParameter("@Description", (object)product.Description ?? DBNull.Value),
                    new SqlParameter("@ApplicabilityType", product.ApplicabilityType),
                    new SqlParameter("@ProductId", product.ProductId),
                }
            );
        }

        /// <summary>
        /// Deletes a product from the inventory by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        public void Delete(int id)
        {
            ExecuteNonQuery(
                "DELETE FROM Products WHERE ProductId = @ProductId",
                new[] { new SqlParameter("@ProductId", id) }
            );
        }
    }
}
