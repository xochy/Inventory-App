using InventoryWpfApp.Models;

namespace InventoryWpfApp.Repositories.Contracts
{
    public interface IGroupRepository : IRepository<Group>
    {
        IEnumerable<Group> GetAllWithEmployeeType();
    }
}
