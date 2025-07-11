using InventoryWpfApp.Models;
using InventoryWpfApp.Repositories.Contracts;
using System.Collections.Generic;

namespace InventoryWpfApp.Repositories.Contracts
{
    public interface IEmployeeRepository : IRepository<Employee> // Inherit from generic IRepository
    {
        // Specific methods for Employee if needed, beyond basic CRUD
        IEnumerable<Employee> GetAllWithGroupAndType();
    }

    //public interface IGroupRepository : IRepository<Group>
    //{
    //    IEnumerable<Group> GetAllWithEmployeeType();
    //}

    //public interface IEmployeeTypeRepository : IRepository<EmployeeType>
    //{
    //    // No specific methods needed for now beyond basic CRUD
    //}
}