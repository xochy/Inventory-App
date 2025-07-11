using InventoryWpfApp.Models;
using InventoryWpfApp.Repositories.Contracts;
using System.Collections.Generic;

namespace InventoryWpfApp.Repositories.Contracts
{
    public interface IInventoryStockRepository : IRepository<InventoryStock>
    {
        IEnumerable<InventoryStock> GetStockDetails();
        void AddOrUpdateStock(int productId, int sizeId, int quantity, int minStockLimit);
        IEnumerable<InventoryStock> GetAvailableSizesForProduct(int productId);
    }
}