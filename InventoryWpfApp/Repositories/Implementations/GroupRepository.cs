using InventoryWpfApp.Models;
using InventoryWpfApp.Repositories.Contracts;
using InventoryWpfApp.Repositories.Helpers;
using Microsoft.Data.SqlClient;

namespace InventoryWpfApp.Repositories.Implementations
{
    /// <summary>
    /// Repository for managing groups.
    /// </summary>
    public class GroupRepository : BaseRepository, IGroupRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The database connection factory.</param>
        public GroupRepository(IDbConnectionFactory connectionFactory)
            : base(connectionFactory) { }

        /// <summary>
        /// Gets all groups.
        /// </summary>
        /// <returns>A list of all groups.</returns>
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
                        groups.Add(
                            new Group
                            {
                                GroupId = reader.GetInt32(reader.GetOrdinal("GroupId")),
                                Name = reader.GetString(reader.GetOrdinal("Group")),
                                EmployeeTypeId = reader.GetInt32(
                                    reader.GetOrdinal("EmployeeTypeId")
                                ),
                            }
                        );
                    }
                }
            }
            return groups;
        }

        /// <summary>
        /// Gets all groups with their employee type information.
        /// </summary>
        /// <returns>A list of all groups with their employee type information.</returns>
        public IEnumerable<Group> GetAllWithEmployeeType()
        {
            var groups = new List<Group>();
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText =
                    @"
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
                        groups.Add(
                            new Group
                            {
                                GroupId = reader.GetInt32(reader.GetOrdinal("GroupId")),
                                Name = reader.GetString(reader.GetOrdinal("Group")),
                                EmployeeTypeId = reader.GetInt32(
                                    reader.GetOrdinal("EmployeeTypeId")
                                ),
                                EmployeeTypeName = reader.GetString(
                                    reader.GetOrdinal("EmployeeTypeName")
                                ),
                            }
                        );
                    }
                }
            }
            return groups;
        }

        /// <summary>
        /// Gets a group by its ID.
        /// </summary>
        /// <param name="id">The ID of the group.</param>
        /// <returns>The group with the specified ID, or null if not found.</returns>
        public Group GetById(int id)
        {
            Group group = null;
            using (var connection = GetConnection())
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT GroupId, [Group], EmployeeTypeId FROM Groups WHERE GroupId = @GroupId";
                command.Parameters.AddWithValue("@GroupId", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        group = new Group
                        {
                            GroupId = reader.GetInt32(reader.GetOrdinal("GroupId")),
                            Name = reader.GetString(reader.GetOrdinal("Group")),
                            EmployeeTypeId = reader.GetInt32(reader.GetOrdinal("EmployeeTypeId")),
                        };
                    }
                }
            }
            return group;
        }

        /// <summary>
        /// Adds a new group.
        /// </summary>
        /// <param name="group">The group to add.</param>
        public void Add(Group group)
        {
            ExecuteNonQuery(
                "INSERT INTO Groups ([Group], EmployeeTypeId) VALUES (@Group, @EmployeeTypeId)",
                new[]
                {
                    new SqlParameter("@Group", group.Name),
                    new SqlParameter("@EmployeeTypeId", group.EmployeeTypeId),
                }
            );
        }

        /// <summary>
        /// Updates an existing group.
        /// </summary>
        /// <param name="group">The group to update.</param>
        public void Update(Group group)
        {
            ExecuteNonQuery(
                "UPDATE Groups SET [Group] = @Group, EmployeeTypeId = @EmployeeTypeId WHERE GroupId = @GroupId",
                new[]
                {
                    new SqlParameter("@Group", group.Name),
                    new SqlParameter("@EmployeeTypeId", group.EmployeeTypeId),
                    new SqlParameter("@GroupId", group.GroupId),
                }
            );
        }

        /// <summary>
        /// Deletes a group by its ID.
        /// </summary>
        /// <param name="id">The ID of the group to delete.</param>
        public void Delete(int id)
        {
            ExecuteNonQuery(
                "DELETE FROM Groups WHERE GroupId = @GroupId",
                new[] { new SqlParameter("@GroupId", id) }
            );
        }
    }
}
