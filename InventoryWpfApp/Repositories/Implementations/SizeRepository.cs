using InventoryWpfApp.Models;
using InventoryWpfApp.Repositories.Contracts;
using InventoryWpfApp.Repositories.Helpers;
using Microsoft.Data.SqlClient;

namespace InventoryWpfApp.Repositories.Implementations
{
    /// <summary>
    /// Repository for managing sizes in the inventory.
    /// </summary>
    public class SizeRepository : BaseRepository, ISizeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SizeRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The database connection factory.</param>
        public SizeRepository(IDbConnectionFactory connectionFactory)
            : base(connectionFactory) { }

        /// <summary>
        /// Gets all sizes.
        /// </summary>
        /// <returns>A list of all sizes in the inventory.</returns>
        public IEnumerable<Size> GetAll()
        {
            var sizes = new List<Size>();
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = "SELECT SizeId, SizeValue, NotationType FROM Sizes";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sizes.Add(
                            new Size
                            {
                                SizeId = reader.GetInt32(reader.GetOrdinal("SizeId")),
                                SizeValue = reader.GetString(reader.GetOrdinal("SizeValue")),
                                NotationType = reader.GetString(reader.GetOrdinal("NotationType")),
                            }
                        );
                    }
                }
            }
            return sizes;
        }

        /// <summary>
        /// Gets a size by its ID.
        /// </summary>
        /// <param name="id">The ID of the size.</param>
        /// <returns>The size with the specified ID, or null if not found.</returns>
        public Size GetById(int id)
        {
            Size size = null;
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT SizeId, SizeValue, NotationType FROM Sizes WHERE SizeId = @SizeId";
                command.Parameters.AddWithValue("@SizeId", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        size = new Size
                        {
                            SizeId = reader.GetInt32(reader.GetOrdinal("SizeId")),
                            SizeValue = reader.GetString(reader.GetOrdinal("SizeValue")),
                            NotationType = reader.GetString(reader.GetOrdinal("NotationType")),
                        };
                    }
                }
            }
            return size;
        }

        /// <summary>
        /// Adds a new size.
        /// </summary>
        /// <param name="size">The size to add.</param>
        public void Add(Size size)
        {
            ExecuteNonQuery(
                "INSERT INTO Sizes (SizeValue, NotationType) VALUES (@SizeValue, @NotationType)",
                new[]
                {
                    new SqlParameter("@SizeValue", size.SizeValue),
                    new SqlParameter("@NotationType", size.NotationType),
                }
            );
        }

        /// <summary>
        /// Updates an existing size.
        /// </summary>
        /// <param name="size">The size to update.</param>
        public void Update(Size size)
        {
            ExecuteNonQuery(
                "UPDATE Sizes SET SizeValue = @SizeValue, NotationType = @NotationType WHERE SizeId = @SizeId",
                new[]
                {
                    new SqlParameter("@SizeValue", size.SizeValue),
                    new SqlParameter("@NotationType", size.NotationType),
                    new SqlParameter("@SizeId", size.SizeId),
                }
            );
        }

        /// <summary>
        /// Deletes a size by its ID.
        /// </summary>
        /// <param name="id">The ID of the size to delete.</param>
        public void Delete(int id)
        {
            ExecuteNonQuery(
                "DELETE FROM Sizes WHERE SizeId = @SizeId",
                new[] { new SqlParameter("@SizeId", id) }
            );
        }
    }
}
