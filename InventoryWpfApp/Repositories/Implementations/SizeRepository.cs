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
    public class SizeRepository : BaseRepository, ISizeRepository
    {
        public SizeRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }

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
                        sizes.Add(new Size
                        {
                            SizeId = reader.GetInt32(reader.GetOrdinal("SizeId")),
                            SizeValue = reader.GetString(reader.GetOrdinal("SizeValue")),
                            NotationType = reader.GetString(reader.GetOrdinal("NotationType"))
                        });
                    }
                }
            }
            return sizes;
        }

        public Size GetById(int id)
        {
            Size size = null;
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = "SELECT SizeId, SizeValue, NotationType FROM Sizes WHERE SizeId = @SizeId";
                command.Parameters.AddWithValue("@SizeId", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        size = new Size
                        {
                            SizeId = reader.GetInt32(reader.GetOrdinal("SizeId")),
                            SizeValue = reader.GetString(reader.GetOrdinal("SizeValue")),
                            NotationType = reader.GetString(reader.GetOrdinal("NotationType"))
                        };
                    }
                }
            }
            return size;
        }

        public void Add(Size size)
        {
            ExecuteNonQuery("INSERT INTO Sizes (SizeValue, NotationType) VALUES (@SizeValue, @NotationType)", new[]
            {
                new SqlParameter("@SizeValue", size.SizeValue),
                new SqlParameter("@NotationType", size.NotationType)
            });
        }

        public void Update(Size size)
        {
            ExecuteNonQuery("UPDATE Sizes SET SizeValue = @SizeValue, NotationType = @NotationType WHERE SizeId = @SizeId", new[]
            {
                new SqlParameter("@SizeValue", size.SizeValue),
                new SqlParameter("@NotationType", size.NotationType),
                new SqlParameter("@SizeId", size.SizeId)
            });
        }

        public void Delete(int id)
        {
            ExecuteNonQuery("DELETE FROM Sizes WHERE SizeId = @SizeId", new[]
            {
                new SqlParameter("@SizeId", id)
            });
        }
    }
}
