using InventoryWpfApp.Models;
using InventoryWpfApp.Repositories.Contracts;
using InventoryWpfApp.Repositories.Helpers;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace InventoryWpfApp.Repositories.Implementations
{
    public class EmployeeRepository : BaseRepository, IEmployeeRepository
    {
        public EmployeeRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }

        public IEnumerable<Employee> GetAll()
        {
            var employees = new List<Employee>();
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = "SELECT EmployeeId, Name, GroupId FROM Employees";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            GroupId = reader.GetInt32(reader.GetOrdinal("GroupId"))
                        });
                    }
                }
            }
            return employees;
        }

        public IEnumerable<Employee> GetAllWithGroupAndType()
        {
            var employees = new List<Employee>();
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT
                        E.EmployeeId,
                        E.Name,
                        E.GroupId,
                        G.[Group] AS GroupName,
                        ET.[Type] AS EmployeeTypeName
                    FROM Employees E
                    JOIN Groups G ON E.GroupId = G.GroupId
                    JOIN EmployeeTypes ET ON G.EmployeeTypeId = ET.EmployeeTypeId";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            GroupId = reader.GetInt32(reader.GetOrdinal("GroupId")),
                            GroupName = reader.GetString(reader.GetOrdinal("GroupName")),
                            EmployeeTypeName = reader.GetString(reader.GetOrdinal("EmployeeTypeName"))
                        });
                    }
                }
            }
            return employees;
        }

        public Employee GetById(int id)
        {
            Employee employee = null;
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = "SELECT EmployeeId, Name, GroupId FROM Employees WHERE EmployeeId = @EmployeeId";
                command.Parameters.AddWithValue("@EmployeeId", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        employee = new Employee
                        {
                            EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            GroupId = reader.GetInt32(reader.GetOrdinal("GroupId"))
                        };
                    }
                }
            }
            return employee;
        }

        public void Add(Employee employee)
        {
            ExecuteNonQuery("INSERT INTO Employees (Name, GroupId) VALUES (@Name, @GroupId)", new[]
            {
                new SqlParameter("@Name", employee.Name),
                new SqlParameter("@GroupId", employee.GroupId)
            });
        }

        public void Update(Employee employee)
        {
            ExecuteNonQuery("UPDATE Employees SET Name = @Name, GroupId = @GroupId WHERE EmployeeId = @EmployeeId", new[]
            {
                new SqlParameter("@Name", employee.Name),
                new SqlParameter("@GroupId", employee.GroupId),
                new SqlParameter("@EmployeeId", employee.EmployeeId)
            });
        }

        public void Delete(int id)
        {
            ExecuteNonQuery("DELETE FROM Employees WHERE EmployeeId = @EmployeeId", new[]
            {
                new SqlParameter("@EmployeeId", id)
            });
        }
    }
}