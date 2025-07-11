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
    public class ProductRepository : BaseRepository, IProductRepository
    {
        public ProductRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }

        public IEnumerable<Product> GetAll()
        {
            var products = new List<Product>();
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = "SELECT ProductId, Name, Description, ApplicabilityType FROM Products";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                            ApplicabilityType = reader.GetString(reader.GetOrdinal("ApplicabilityType"))
                        });
                    }
                }
            }
            return products;
        }

        public Product GetById(int id)
        {
            Product product = null;
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = "SELECT ProductId, Name, Description, ApplicabilityType FROM Products WHERE ProductId = @ProductId";
                command.Parameters.AddWithValue("@ProductId", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        product = new Product
                        {
                            ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                            ApplicabilityType = reader.GetString(reader.GetOrdinal("ApplicabilityType"))
                        };
                    }
                }
            }
            return product;
        }

        public void Add(Product product)
        {
            ExecuteNonQuery("INSERT INTO Products (Name, Description, ApplicabilityType) VALUES (@Name, @Description, @ApplicabilityType)", new[]
            {
                new SqlParameter("@Name", product.Name),
                new SqlParameter("@Description", (object)product.Description ?? DBNull.Value),
                new SqlParameter("@ApplicabilityType", product.ApplicabilityType)
            });
        }

        public void Update(Product product)
        {
            ExecuteNonQuery("UPDATE Products SET Name = @Name, Description = @Description, ApplicabilityType = @ApplicabilityType WHERE ProductId = @ProductId", new[]
            {
                new SqlParameter("@Name", product.Name),
                new SqlParameter("@Description", (object)product.Description ?? DBNull.Value),
                new SqlParameter("@ApplicabilityType", product.ApplicabilityType),
                new SqlParameter("@ProductId", product.ProductId)
            });
        }

        public void Delete(int id)
        {
            ExecuteNonQuery("DELETE FROM Products WHERE ProductId = @ProductId", new[]
            {
                new SqlParameter("@ProductId", id)
            });
        }
    }
}
