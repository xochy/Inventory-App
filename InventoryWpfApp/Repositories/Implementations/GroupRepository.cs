using InventoryWpfApp.Models;
using InventoryWpfApp.Repositories.Contracts;
using InventoryWpfApp.Repositories.Helpers;
using Microsoft.Data.SqlClient;

namespace InventoryWpfApp.Repositories.Implementations
{
    public class GroupRepository : BaseRepository, IGroupRepository
    {
        public GroupRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }

        public IEnumerable<Group> GetAll()
        {
            var groups = new List<Group>();
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = "SELECT GroupId, [Group], EmployeeTypeId FROM Groups";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        groups.Add(new Group
                        {
                            GroupId = reader.GetInt32(reader.GetOrdinal("GroupId")),
                            Name = reader.GetString(reader.GetOrdinal("Group")),
                            EmployeeTypeId = reader.GetInt32(reader.GetOrdinal("EmployeeTypeId"))
                        });
                    }
                }
            }
            return groups;
        }

        public IEnumerable<Group> GetAllWithEmployeeType()
        {
            var groups = new List<Group>();
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT
                        G.GroupId,
                        G.[Group],
                        G.EmployeeTypeId,
                        ET.[Type] AS EmployeeTypeName
                    FROM Groups G
                    JOIN EmployeeTypes ET ON G.EmployeeTypeId = ET.EmployeeTypeId";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        groups.Add(new Group
                        {
                            GroupId = reader.GetInt32(reader.GetOrdinal("GroupId")),
                            Name = reader.GetString(reader.GetOrdinal("Group")),
                            EmployeeTypeId = reader.GetInt32(reader.GetOrdinal("EmployeeTypeId")),
                            EmployeeTypeName = reader.GetString(reader.GetOrdinal("EmployeeTypeName"))
                        });
                    }
                }
            }
            return groups;
        }

        public Group GetById(int id)
        {
            Group group = null;
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = "SELECT GroupId, [Group], EmployeeTypeId FROM Groups WHERE GroupId = @GroupId";
                command.Parameters.AddWithValue("@GroupId", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        group = new Group
                        {
                            GroupId = reader.GetInt32(reader.GetOrdinal("GroupId")),
                            Name = reader.GetString(reader.GetOrdinal("Group")),
                            EmployeeTypeId = reader.GetInt32(reader.GetOrdinal("EmployeeTypeId"))
                        };
                    }
                }
            }
            return group;
        }

        public void Add(Group group)
        {
            ExecuteNonQuery("INSERT INTO Groups ([Group], EmployeeTypeId) VALUES (@Group, @EmployeeTypeId)", new[]
            {
                new SqlParameter("@Group", group.Name),
                new SqlParameter("@EmployeeTypeId", group.EmployeeTypeId)
            });
        }

        public void Update(Group group)
        {
            ExecuteNonQuery("UPDATE Groups SET [Group] = @Group, EmployeeTypeId = @EmployeeTypeId WHERE GroupId = @GroupId", new[]
            {
                new SqlParameter("@Group", group.Name),
                new SqlParameter("@EmployeeTypeId", group.EmployeeTypeId),
                new SqlParameter("@GroupId", group.GroupId)
            });
        }

        public void Delete(int id)
        {
            ExecuteNonQuery("DELETE FROM Groups WHERE GroupId = @GroupId", new[]
            {
                new SqlParameter("@GroupId", id)
            });
        }
    }
}
