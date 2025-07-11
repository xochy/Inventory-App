using InventoryWpfApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryWpfApp.Repositories.Contracts
{
    public interface IGroupRepository : IRepository<Group>
    {
        IEnumerable<Group> GetAllWithEmployeeType();
    }
}
