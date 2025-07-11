using Microsoft.Data.SqlClient;
using System.Data;

namespace InventoryWpfApp.Repositories.Helpers
{
    /// <summary>
    /// A factory for creating database connections.
    /// Implements IDbConnectionFactory for testability and flexibility.
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// Creates and returns a new database connection.
        /// </summary>
        /// <returns>An opened SqlConnection object.</returns>
        IDbConnection CreateConnection();
    }

    /// <summary>
    /// Concrete implementation of IDbConnectionFactory for SQL Server.
    /// </summary>
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        // IMPORTANT: Replace "YOUR_SERVER" with your SQL Server instance name
        // Example: "Data Source=.\\SQLEXPRESS;Initial Catalog=InventoryTechnicalTestDB;Integrated Security=True;TrustServerCertificate=True;"
        private readonly string _connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=InventoryTechnicalTestDB;Integrated Security=True;TrustServerCertificate=True;";

        /// <summary>
        /// Creates and opens a new SqlConnection.
        /// </summary>
        /// <returns>An opened SqlConnection.</returns>
        public IDbConnection CreateConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}
