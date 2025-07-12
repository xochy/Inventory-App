using InventoryWpfApp.Models;

namespace InventoryWpfApp.Repositories.Contracts
{
    public interface IInventoryStockRepository : IRepository<InventoryStock>
    {
        IEnumerable<InventoryStock> GetStockDetails();
        void AddOrUpdateStock(int productId, int sizeId, int quantity, int minStockLimit);
        IEnumerable<InventoryStock> GetAvailableSizesForProduct(int productId);
    }
}