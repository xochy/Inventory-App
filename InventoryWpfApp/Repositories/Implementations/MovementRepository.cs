using System.Data;
using InventoryWpfApp.Models;
using InventoryWpfApp.Repositories.Contracts;
using InventoryWpfApp.Repositories.Helpers;
using Microsoft.Data.SqlClient;

namespace InventoryWpfApp.Repositories.Implementations
{
    /// <summary>
    /// Repository for managing movements in the inventory.
    /// </summary>
    public class MovementRepository : BaseRepository, IMovementRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MovementRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The database connection factory.</param>
        public MovementRepository(IDbConnectionFactory connectionFactory)
            : base(connectionFactory) { }

        /// <summary>
        /// Gets all movements in the inventory.
        /// </summary>
        /// <returns>A list of all inventory movements.</returns>
        public IEnumerable<Movement> GetAllMovements()
        {
            var movements = new List<Movement>();
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT * FROM vw_InventoryWerehouseMovements ORDER BY MovementDate DESC";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        movements.Add(
                            new Movement
                            {
                                MovementId = reader.GetInt32(reader.GetOrdinal("MovementId")),
                                MovementDate = reader.GetDateTime(
                                    reader.GetOrdinal("MovementDate")
                                ),
                                MovementType = reader.GetString(reader.GetOrdinal("MovementType")),
                                QuantityMoved = reader.GetInt32(reader.GetOrdinal("QuantityMoved")),
                                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                                ProductDescription = reader.GetString(
                                    reader.GetOrdinal("ProductDescription")
                                ),
                                ProductSize = reader.GetString(reader.GetOrdinal("ProductSize")),
                                SizeNotation = reader.IsDBNull(reader.GetOrdinal("SizeNotation"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("SizeNotation")),
                                RemainingStock = reader.GetInt32(
                                    reader.GetOrdinal("RemainingStock")
                                ),
                                MinStockLimit = reader.GetInt32(reader.GetOrdinal("MinStockLimit")),
                                EmployeeName = reader.IsDBNull(reader.GetOrdinal("EmployeeName"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("EmployeeName")),
                                EmployeeGroup = reader.IsDBNull(reader.GetOrdinal("EmployeeGroup"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("EmployeeGroup")),
                                EmployeeType = reader.IsDBNull(reader.GetOrdinal("EmployeeType"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("EmployeeType")),
                                Notes = reader.IsDBNull(reader.GetOrdinal("Notes"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("Notes")),
                            }
                        );
                    }
                }
            }
            return movements;
        }

        /// <summary>
        /// Registers a delivery in the inventory.
        /// </summary>
        /// <param name="inventoryStockId">The ID of the inventory stock item.</param>
        /// <param name="employeeId">The ID of the employee responsible for the delivery.</param>
        /// <param name="quantity">The quantity of items delivered.</param>
        public void RegisterDelivery(int inventoryStockId, int employeeId, int quantity)
        {
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = "sp_InventoryRegisterDelivery";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@InventoryStockId", inventoryStockId);
                command.Parameters.AddWithValue("@EmployeeId", employeeId);
                command.Parameters.AddWithValue("@Quantity", quantity);

                try
                {
                    //connection.Open(); // Connection is already open by GetConnection
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    // Rethrow the exception to be handled by the ViewModel/UI
                    throw new Exception(
                        "Database error during delivery registration: " + ex.Message,
                        ex
                    );
                }
            }
        }
    }
}
