using InventoryWpfApp.Models;

namespace InventoryWpfApp.Repositories.Contracts
{
    public interface IMovementRepository
    {
        IEnumerable<Movement> GetAllMovements();
        void RegisterDelivery(int inventoryStockId, int employeeId, int quantity);
    }
}
