using InventoryWpfApp.Repositories.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InventoryWpfApp.Repositories.Implementations
{
    /// <summary>
    /// Base class for repositories to manage database connections.
    /// </summary>
    public abstract class BaseRepository
    {
        protected readonly IDbConnectionFactory _connectionFactory;

        public BaseRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        protected IDbConnection GetConnection()
        {
            return _connectionFactory.CreateConnection();
        }

        protected T ExecuteScalar<T>(string query, SqlParameter[] parameters = null)
        {
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = query;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                return (T)command.ExecuteScalar();
            }
        }

        protected int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = query;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                return command.ExecuteNonQuery();
            }
        }
    }
}