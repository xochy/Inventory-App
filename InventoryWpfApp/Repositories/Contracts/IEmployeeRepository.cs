using InventoryWpfApp.Models;

namespace InventoryWpfApp.Repositories.Contracts
{
    public interface IEmployeeRepository : IRepository<Employee> // Inherit from generic IRepository
    {
        // Specific methods for Employee if needed, beyond basic CRUD
        IEnumerable<Employee> GetAllWithGroupAndType();
    }
}