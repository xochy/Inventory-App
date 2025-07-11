using InventoryWpfApp.Models;
using InventoryWpfApp.Repositories.Contracts;
using System.Collections.Generic;

namespace InventoryWpfApp.Repositories.Contracts
{
    public interface ISizeRepository : IRepository<Size>
    {
        // No specific methods needed for now beyond basic CRUD
    }
}