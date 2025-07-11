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
    public class EmployeeTypeRepository : BaseRepository, IEmployeeTypeRepository
    {
        public EmployeeTypeRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }

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
                        employeeTypes.Add(new EmployeeType
                        {
                            EmployeeTypeId = reader.GetInt32(reader.GetOrdinal("EmployeeTypeId")),
                            TypeName = reader.GetString(reader.GetOrdinal("Type"))
                        });
                    }
                }
            }
            return employeeTypes;
        }

        public EmployeeType GetById(int id)
        {
            EmployeeType employeeType = null;
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = "SELECT EmployeeTypeId, [Type] FROM EmployeeTypes WHERE EmployeeTypeId = @EmployeeTypeId";
                command.Parameters.AddWithValue("@EmployeeTypeId", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        employeeType = new EmployeeType
                        {
                            EmployeeTypeId = reader.GetInt32(reader.GetOrdinal("EmployeeTypeId")),
                            TypeName = reader.GetString(reader.GetOrdinal("Type"))
                        };
                    }
                }
            }
            return employeeType;
        }

        public void Add(EmployeeType employeeType)
        {
            ExecuteNonQuery("INSERT INTO EmployeeTypes ([Type]) VALUES (@Type)", new[]
            {
                new SqlParameter("@Type", employeeType.TypeName)
            });
        }

        public void Update(EmployeeType employeeType)
        {
            ExecuteNonQuery("UPDATE EmployeeTypes SET [Type] = @Type WHERE EmployeeTypeId = @EmployeeTypeId", new[]
            {
                new SqlParameter("@Type", employeeType.TypeName),
                new SqlParameter("@EmployeeTypeId", employeeType.EmployeeTypeId)
            });
        }

        public void Delete(int id)
        {
            ExecuteNonQuery("DELETE FROM EmployeeTypes WHERE EmployeeTypeId = @EmployeeTypeId", new[]
            {
                new SqlParameter("@EmployeeTypeId", id)
            });
        }
    }
}
