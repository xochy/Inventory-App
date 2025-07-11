using InventoryWpfApp.Models;
using InventoryWpfApp.Repositories.Contracts;
using InventoryWpfApp.Repositories.Helpers;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryWpfApp.Repositories.Implementations
{
    public class MovementRepository : BaseRepository, IMovementRepository
    {
        public MovementRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }

        public IEnumerable<Movement> GetAllMovements()
        {
            var movements = new List<Movement>();
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM vw_MovimientosAlmacen ORDER BY MovementDate DESC";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        movements.Add(new Movement
                        {
                            MovementId = reader.GetInt32(reader.GetOrdinal("MovementId")),
                            MovementDate = reader.GetDateTime(reader.GetOrdinal("MovementDate")),
                            MovementType = reader.GetString(reader.GetOrdinal("MovementType")),
                            QuantityMoved = reader.GetInt32(reader.GetOrdinal("QuantityMoved")),
                            InventoryStockId = reader.GetInt32(reader.GetOrdinal("InventoryStockId")),
                            ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                            ProductSize = reader.GetString(reader.GetOrdinal("ProductSize")),
                            EmployeeId = reader.IsDBNull(reader.GetOrdinal("EmployeeId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            EmployeeName = reader.IsDBNull(reader.GetOrdinal("EmployeeName")) ? null : reader.GetString(reader.GetOrdinal("EmployeeName")),
                            EmployeeGroup = reader.IsDBNull(reader.GetOrdinal("EmployeeGroup")) ? null : reader.GetString(reader.GetOrdinal("EmployeeGroup")),
                            EmployeeType = reader.IsDBNull(reader.GetOrdinal("EmployeeType")) ? null : reader.GetString(reader.GetOrdinal("EmployeeType")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes"))
                        });
                    }
                }
            }
            return movements;
        }

        public void RegisterDelivery(int inventoryStockId, int employeeId, int quantity)
        {
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = "sp_RegistrarEntregaInventario";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@InventoryStockId", inventoryStockId);
                command.Parameters.AddWithValue("@EmployeeId", employeeId);
                command.Parameters.AddWithValue("@Quantity", quantity);

                try
                {
                    connection.Open(); // Connection is already open by GetConnection, but explicit for clarity
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    // Rethrow the exception to be handled by the ViewModel/UI
                    throw new Exception("Database error during delivery registration: " + ex.Message, ex);
                }
            }
        }
    }
}
