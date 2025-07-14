using InventoryWpfApp.Models;
using InventoryWpfApp.Repositories.Contracts;
using InventoryWpfApp.Repositories.Helpers;
using Microsoft.Data.SqlClient;

namespace InventoryWpfApp.Repositories.Implementations
{
    /// <summary>
    /// Repository for managing employee types.
    /// </summary>
    public class EmployeeTypeRepository : BaseRepository, IEmployeeTypeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeTypeRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The database connection factory.</param>
        public EmployeeTypeRepository(IDbConnectionFactory connectionFactory)
            : base(connectionFactory) { }

        /// <summary>
        /// Gets all employee types.
        /// </summary>
        /// <returns>A list of all employee types.</returns>
        public IEnumerable<EmployeeType> GetAll()
        {
            var employeeTypes = new List<EmployeeType>();
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = "SELECT EmployeeTypeId, [Type] FROM EmployeeTypes";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employeeTypes.Add(
                            new EmployeeType
                            {
                                EmployeeTypeId = reader.GetInt32(
                                    reader.GetOrdinal("EmployeeTypeId")
                                ),
                                TypeName = reader.GetString(reader.GetOrdinal("Type")),
                            }
                        );
                    }
                }
            }
            return employeeTypes;
        }

        /// <summary>
        /// Gets an employee type by its ID.
        /// </summary>
        /// <param name="id">The ID of the employee type.</param>
        /// <returns>The employee type with the specified ID, or null if not found.</returns>
        public EmployeeType GetById(int id)
        {
            EmployeeType employeeType = null;
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT EmployeeTypeId, [Type] FROM EmployeeTypes WHERE EmployeeTypeId = @EmployeeTypeId";
                command.Parameters.AddWithValue("@EmployeeTypeId", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        employeeType = new EmployeeType
                        {
                            EmployeeTypeId = reader.GetInt32(reader.GetOrdinal("EmployeeTypeId")),
                            TypeName = reader.GetString(reader.GetOrdinal("Type")),
                        };
                    }
                }
            }
            return employeeType;
        }

        /// <summary>
        /// Adds a new employee type.
        /// </summary>
        /// <param name="employeeType">The employee type to add.</param>
        public void Add(EmployeeType employeeType)
        {
            ExecuteNonQuery(
                "INSERT INTO EmployeeTypes ([Type]) VALUES (@Type)",
                new[] { new SqlParameter("@Type", employeeType.TypeName) }
            );
        }

        /// <summary>
        /// Updates an existing employee type.
        /// </summary>
        /// <param name="employeeType">The employee type to update.</param>
        public void Update(EmployeeType employeeType)
        {
            ExecuteNonQuery(
                "UPDATE EmployeeTypes SET [Type] = @Type WHERE EmployeeTypeId = @EmployeeTypeId",
                new[]
                {
                    new SqlParameter("@Type", employeeType.TypeName),
                    new SqlParameter("@EmployeeTypeId", employeeType.EmployeeTypeId),
                }
            );
        }

        /// <summary>
        /// Deletes an employee type by its ID.
        /// </summary>
        /// <param name="id">The ID of the employee type to delete.</param>
        public void Delete(int id)
        {
            ExecuteNonQuery(
                "DELETE FROM EmployeeTypes WHERE EmployeeTypeId = @EmployeeTypeId",
                new[] { new SqlParameter("@EmployeeTypeId", id) }
            );
        }
    }
}
