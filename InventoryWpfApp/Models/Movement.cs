using System;

namespace InventoryWpfApp.Models
{
    public class Movement
    {
        public int MovementId { get; set; }
        public DateTime MovementDate { get; set; }
        public string MovementType { get; set; }
        public int QuantityMoved { get; set; }
        public int InventoryStockId { get; set; }
        public string ProductName { get; set; } // From vw_InventoryWerehouseMovements
        public string ProductSize { get; set; } // From vw_InventoryWerehouseMovements
        public int? EmployeeId { get; set; } // Nullable if not always involved
        public string EmployeeName { get; set; } // From vw_InventoryWerehouseMovements
        public string EmployeeGroup { get; set; } // From vw_InventoryWerehouseMovements
        public string EmployeeType { get; set; } // From vw_InventoryWerehouseMovements
        public string Notes { get; set; }
    }
}